#tool "nuget:?package=GitVersion.CommandLine&version=5.6.3"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"
#addin "nuget:?package=Cake.DependenciesAnalyser&version=2.0.0"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Debug");
var verbosity = Argument<string>("verbosity", "Minimal");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var sourceDir = Directory("./src");
var testsDir = Directory("./tests");

var solutions = GetFiles("./**/*.sln");

GitVersion gitVersion = null;

// USED TO CREATE ZIP PACKAGES
var createPackage = false;

// BUILD OUTPUT DIRECTORIES
var artifactsDir = Directory("./artifacts");

// VERBOSITY
var msBuildVerbosity = Cake.Core.Diagnostics.Verbosity.Diagnostic;
if (!Enum.TryParse(verbosity, true, out msBuildVerbosity))
{
	Warning(
		"Verbosity could not be parsed into type 'Cake.Core.Diagnostics.Verbosity'. Defaulting to {0}",
		msBuildVerbosity);
}

///////////////////////////////////////////////////////////////////////////////
// COMMON FUNCTION DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

void Test(string testDllGlob)
{
	var settings = new XUnit2Settings();

	Information("Testing '{0}'...",  testDllGlob);
	XUnit2(testDllGlob, settings);
	Information("'{0}' has been tested.", testDllGlob);
}

void Build(PlatformTarget platformTarget)
{
	var settings = new MSBuildSettings
	{
		Verbosity = msBuildVerbosity,
		Configuration = configuration,
		PlatformTarget = platformTarget,
		ToolVersion = MSBuildToolVersion.VS2017
	};

	foreach(var solution in solutions)
	{
		Information("Building '{0}'...", solution.FullPath);
		MSBuild(solution.FullPath, settings);
		Information("'{0}' has been built.", solution.FullPath);
	}
}

void Pack(string directory, string outputFile)
{
	var directoryToClean = string.Format("{0}*.pdb", directory);

	Information("Cleaning '{0}'...", directoryToClean);
	DeleteFiles(directoryToClean);
	Information("'{0}' has been cleaned.", directoryToClean);

	directoryToClean = string.Format("{0}*.xml", directory);

	Information("Cleaning '{0}'...", directoryToClean);
	DeleteFiles(directoryToClean);
	Information("'{0}' has been cleaned.", directoryToClean);

	Information("Compressing '{0}' to '{1}'...", directory, outputFile);
	Zip(directory, outputFile);
	Information("'{0}' has been compressed to '{1}'.", directory, outputFile);
}

void CopyPlugin(string source, string destination)
{
	var directoryToClean = string.Format("{0}*.pdb", source);

	Information("Cleaning '{0}'...", directoryToClean);
	DeleteFiles(directoryToClean);
	Information("'{0}' has been cleaned.", directoryToClean);

	directoryToClean = string.Format("{0}*.xml", source);

	Information("Cleaning '{0}'...", directoryToClean);
	DeleteFiles(directoryToClean);
	Information("'{0}' has been cleaned.", directoryToClean);

	Information("Copying '{0}' to '{1}'...", source, destination);
	CopyDirectory(source, destination);
	Information("'{0}' has been copied to '{1}'.", source, destination);
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	EnsureDirectoryExists(artifactsDir);
	Information("Running tasks...");
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Description("Cleans all directories that are used during the build process.")
	.Does(() =>
	{
		foreach(var solution in solutions)
		{
			Information("Cleaning {0}", solution.FullPath);
			CleanDirectories(sourceDir.Path + "/**/bin/*");
			CleanDirectories(sourceDir.Path + "/**/obj/*");
			CleanDirectories(testsDir.Path + "/**/bin/*");
			CleanDirectories(testsDir.Path + "/**/obj/*");
			Information("{0} was clean.", solution.FullPath);
		}

		CleanDirectory(artifactsDir);
	});


Task("Restore")
	.Description("Restores all the NuGet packages that are used by the specified solution.")
	.Does(() =>
	{
		var settings = new NuGetRestoreSettings
		{
			MSBuildVersion = NuGetMSBuildVersion.MSBuild14,
			NoCache = true,
			Verbosity = NuGetVerbosity.Normal
		};

		foreach(var solution in solutions)
		{
			Information("Restoring NuGet packages for '{0}'...", solution);
			NuGetRestore(solution, settings);
			Information("NuGet packages restored for '{0}'.", solution);
		}
	});

Task("SemVer")
	.Description("Applies the SemVer to all the different parts of the project.")
	.Does(() =>
	{
		var settings = new GitVersionSettings
		{
			UpdateAssemblyInfo = true
		};

		Information("Applying SemVer.");
		gitVersion = GitVersion(settings);
		Information("SemVer has been applied.");
	});

Task("Build")
	.Description("Builds all the different parts of the project.")
	.Does(() =>
	{
		Build(PlatformTarget.MSIL);
		Build(PlatformTarget.x64);
	});

Task("Test-Unit")
	.Description("Runs all your unit tests, using dotnet CLI.")
	.Does(() =>
	{
		var unitTestsProjects = $"./tests/**/bin/{configuration}/**/*.Tests.Unit.dll";
		Test(unitTestsProjects);

		unitTestsProjects = $"./tests/**/bin/x64/{configuration}/**/*.Tests.Unit.dll";
		Test(unitTestsProjects);
	});

Task("Dependencies-Analyse")
	.Description("Runs the Dependencies Analyser on the solution.")
	.Does(() =>
	{
		var settings = new DependenciesAnalyserSettings
		{
			Folder = "./src/"
		};

		Information("Analysing dependencies on folder '{0}'...", sourceDir.Path);
		AnalyseDependencies(settings);
		Information("'{0}' dependencies for the projects had been analysed.", sourceDir.Path);
	});

Task("AppVeyor-Pack")
	.Description("Prepares to pack the project, using AppVeyor.")
	.Does(() =>
	{
		var tagBuildEnvVar = EnvironmentVariable("APPVEYOR_REPO_TAG");
		bool.TryParse(tagBuildEnvVar, out createPackage);
	});

Task("Local-Pack")
	.Description("Prepares to pack the project, using local environment.")
	.Does(() =>
	{
		createPackage = true;
	});

Task("Pack")
	.Description("Packs all the different parts of the project.")
	.Does(() =>
	{
		if(!createPackage)
		{
			Information("Skipping the Zip pack step.");
			return;
		}

		var patternFolder = "./src/NppXmlTreeviewPlugin/bin/{0}/";
		var patternOutput = "{0}/NppXMLTreeViewPlugin_{1}.zip";
		var platform = "x86";

		Pack(
		  string.Format(patternFolder, configuration),
		  string.Format(patternOutput, artifactsDir.Path, platform));

		patternFolder = "./src/NppXmlTreeviewPlugin/bin/x64/{0}/";
		platform = "x64";

		Pack(
		  string.Format(patternFolder, configuration),
		  string.Format(patternOutput, artifactsDir.Path, platform));
	});

Task("Local-Deploy")
	.Description("Deploys the puglin, using local environment.")
	.Does(() =>
	{
		var patternFolder = "./src/NppXmlTreeviewPlugin/bin/{0}/";

		CopyPlugin(
		  string.Format(patternFolder, configuration),
		  @"C:\Program Files (x86)\Notepad++\plugins");

		patternFolder = "./src/NppXmlTreeviewPlugin/bin/x64/{0}/";

		CopyPlugin(
		  string.Format(patternFolder, configuration),
		  @"C:\Program Files\Notepad++\plugins");
	});

///////////////////////////////////////////////////////////////////////////////
// COMBINATIONS - let's make life easier...
///////////////////////////////////////////////////////////////////////////////

Task("Build+Test")
	.Description("First runs Build, then Test targets.")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("SemVer")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit")
	.Does(() => { Information("Ran Build+Test target"); });

Task("Rebuild")
	.Description("Runs a full Clean+Restore+Build build.")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("SemVer")
	.IsDependentOn("Build")
	.Does(() => { Information("Rebuilt everything"); });

Task("Test-All")
	.Description("Runs all your tests.")
	.IsDependentOn("Test-Unit")
	.Does(() => { Information("Tested everything"); });

Task("Analyse")
	.Description("Analyse the solution.")
	.IsDependentOn("Build+Test")
	.IsDependentOn("Dependencies-Analyse")
	.Does(() => { Information("Analyses done"); });

Task("LocalPack")
	.Description("Runs locally.")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("SemVer")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit")
	//.IsDependentOn("Dependencies-Analyse")
	.IsDependentOn("Local-Pack")
	.IsDependentOn("Pack")
	.Does(() => { Information("Everything is done! Well done Local Pack."); });

Task("LocalDeploy")
	.Description("Runs locally.")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("SemVer")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit")
	//.IsDependentOn("Dependencies-Analyse")
	.IsDependentOn("Local-Deploy")
	.Does(() => { Information("Everything is done! Well done Local Deploy."); });

Task("AppVeyor")
	.Description("Runs on AppVeyor after 'merging master'.")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("SemVer")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Unit")
	//.IsDependentOn("Dependencies-Analyse")
	.IsDependentOn("AppVeyor-Pack")
	.IsDependentOn("Pack")
	.Does(() => { Information("Everything is done! Well done AppVeyor."); });

///////////////////////////////////////////////////////////////////////////////
// DEFAULT TARGET
///////////////////////////////////////////////////////////////////////////////

Task("Default")
	.Description("This is the default task which will run if no specific target is passed in.")
	.IsDependentOn("Build+Test")
	.Does(() => { Warning("No 'Target' was passed in, so we ran the 'Build+Test' operation."); });

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);