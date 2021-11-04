﻿/*

MIT License

Copyright (c) Jeff Campbell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace JCMG.Genesis.Editor
{

	/// <summary>
	/// An editor class for managing project and user preferences for the Genesis library.
	/// </summary>
	internal static class GenesisPreferences
	{
		/// <summary>
		/// The installation folder for the Genesis command-line executable.
		/// </summary>
		public static string GenesisCLIInstallationFolder
		{
			get
			{
				if (_genesisCLIInstallationFolder == null)
				{
					_genesisCLIInstallationFolder = ProjectSettings.Get(
						GENESIS_INSTALLATION_PREF,
						SettingsScope.Project,
						string.Empty);
				}

				return _genesisCLIInstallationFolder;
			}
			set
			{
				_genesisCLIInstallationFolder = value;
				ProjectSettings.Set(GENESIS_INSTALLATION_PREF, value);
			}
		}

		/// <summary>
		/// Returns true if the code generation should execute as a dry run, otherwise false.
		/// </summary>
		public static bool ExecuteDryRun
		{
			get
			{
				if (!_executeDryRun.HasValue)
				{
					_executeDryRun = EditorPrefs.GetBool(ENABLE_DRY_RUN_PREF, ENABLE_DRY_RUN_DEFAULT);
				}

				return _executeDryRun.Value;
			}
			set
			{
				_executeDryRun = value;
				EditorPrefs.SetBool(ENABLE_DRY_RUN_PREF, value);
			}
		}

		/// <summary>
		/// Returns true if the code generation should be executed with verbose logging, otherwise false.
		/// </summary>
		public static bool EnableVerboseLogging
		{
			get
			{
				if (!_enableVerboseLogging.HasValue)
				{
					_enableVerboseLogging = EditorPrefs.GetBool(ENABLE_VERBOSE_LOGGING_PREF, ENABLE_VERBOSE_LOGGING_DEFAULT);
				}

				return _enableVerboseLogging.Value;
			}
			set
			{
				_enableVerboseLogging = value;
				EditorPrefs.SetBool(ENABLE_VERBOSE_LOGGING_PREF, value);
			}
		}

		/// <summary>
		/// Returns true if code generation should loading of assemblies in <see cref="System.AppDomain.CurrentDomain"/>,
		/// otherwise false.
		/// </summary>
		public static bool LoadCurrentDomainAssemblies
		{
			get
			{
				if (!_loadCurrentDomainAssemblies.HasValue)
				{
					_loadCurrentDomainAssemblies = ProjectSettings.Get<bool>(LOAD_CURRENT_DOMAIN_ASSEMBLIES_PREF);
				}

				return _loadCurrentDomainAssemblies.Value;
			}
			set
			{
				_loadCurrentDomainAssemblies = value;
				ProjectSettings.Set(LOAD_CURRENT_DOMAIN_ASSEMBLIES_PREF, value);
			}
		}

		/// <summary>
		/// Returns true if code generation should force loading of unsafe plugins (out of date plugins), otherwise
		/// false.
		/// </summary>
		public static bool LoadUnsafePlugins
		{
			get
			{
				if (!_loadUnsafePlugins.HasValue)
				{
					_loadUnsafePlugins = ProjectSettings.Get<bool>(LOAD_UNSAFE_PLUGINS_PREF);
				}

				return _loadUnsafePlugins.Value;
			}
			set
			{
				_loadUnsafePlugins = value;
				ProjectSettings.Set(LOAD_UNSAFE_PLUGINS_PREF, value);
			}
		}

		/// <summary>
		/// The project <see cref="Settings"/> instance for Genesis for this project.
		/// </summary>
		private static Settings ProjectSettings
		{
			get
			{
				if (_PROJECT_SETTINGS == null)
				{
					_PROJECT_SETTINGS = new Settings(PACKAGE_NAME, SETTINGS_NAME);
				}

				return _PROJECT_SETTINGS;
			}
		}

		private static Settings _PROJECT_SETTINGS;

		// Project Settings
		private const string PACKAGE_NAME = "com.jeffcampbellmakesgames.genesis";
		private const string SETTINGS_NAME = "GenesisSettings";

		// UI
		private const string PREFERENCES_TITLE_PATH = "Preferences/Genesis";
		private const string PROJECT_TITLE_PATH = "Project/Genesis";

		private const string USER_PREFERENCES_HEADER = "User Preferences";
		private const string PROJECT_REFERENCES_HEADER = "Project Preferences";
		private const string PLUGIN_SETTINGS_HEADER = "Genesis Plugin Settings";
		private const string PLUGIN_INSTALLERS_HEADER = "Plugin Installers";

		// Labels
		private const string ENABLE_DRY_RUN_LABEL = "Enable Dry Run";
		private const string ENABLE_VERBOSE_LOGGING_LABEL = "Enable Verbose Logging";
		private const string GENESIS_CLI_INSTALL_FOLDER_LABEL = "Installation Folder";

		// Descriptions
		private const string ENABLE_DRY_RUN_DESCRIPTION
			= "If enabled, the code generation process when executed will not write any generated code to disk.";

		private const string ENABLE_VERBOSE_LOGGING_DESCRIPTION =
			"If enabled, additional information will be logged to the console.";

		private const string GENESIS_CLI_DIRECTORY_DESCRIPTION
			= "The location of the Genesis.CLI command-line executable and related files. These should be extracted " +
			  "from the \"Genesis.CLI.zip\" included with this framework to a folder outside of the Assets folder.";

		private const string NONE_FOUND_DESCRIPTION = "None Found";

		// Warnings
		private const string DIRECTORY_DOES_NOT_EXIST_WARNING =
			"Cannot find this directory, please set this path to a valid path containing " +
			"the Genesis.CLI exectable.";

		private const string GENESIS_EXE_DOES_NOT_EXIST_WARNING =
			"Cannot find the Genesis.CLI executable contents at this installation path, please set this path to a " +
			"valid path containing the Genesis.CLI exectable.";

		private const string PLUGINS_DIRECTORY_DOES_NOT_EXIST_WARNING =
			"Cannot find this directory, please set this path to a valid path containing the plugins.";

		// Titles
		private const string GENESIS_CLI_FOLDER_SELECT_TITLE = "Select Genesis CLI Install Folder";

		// Searchable Fields
		private static readonly string[] KEYWORDS =
		{
			"JCMG",
			"genesis",
			"code generation",
			"code gen",
			"code",
			"generation",
			"gen"
		};

		// Project Editor Preferences
		private const string GENESIS_INSTALLATION_PREF = "Genesis.InstallationFolder";

		// User Editor Preferences
		private const string ENABLE_DRY_RUN_PREF = "Genesis.DryRun";
		private const string ENABLE_VERBOSE_LOGGING_PREF = "Genesis.IsVerbose";
		private const string LOAD_CURRENT_DOMAIN_ASSEMBLIES_PREF = "Genesis.LoadCurrentDomainAssemblies";
		private const string LOAD_UNSAFE_PLUGINS_PREF = "Genesis.LoadUnsafePlugins";

		private const bool ENABLE_DRY_RUN_DEFAULT = false;
		private const bool ENABLE_VERBOSE_LOGGING_DEFAULT = true;
		private const bool LOAD_UNSAFE_PLUGINS_DEFAULT = false;

		// Cacheable Prefs
		private static bool? _executeDryRun;
		private static bool? _enableVerboseLogging;
		private static bool? _loadCurrentDomainAssemblies;
		private static bool? _loadUnsafePlugins;
		private static string _genesisCLIInstallationFolder;

		#region SettingsProvider and EditorGUI

		[SettingsProvider]
		private static SettingsProvider CreatePersonalPreferenceSettingsProvider()
		{
			return new SettingsProvider(PREFERENCES_TITLE_PATH, SettingsScope.User)
			{
				guiHandler = DrawPersonalPrefsGUI,
				keywords = KEYWORDS
			};
		}

		[SettingsProvider]
		private static SettingsProvider CreateSettingsProvider()
		{
			return new SettingsProvider(PROJECT_TITLE_PATH, SettingsScope.Project)
			{
				guiHandler = DrawProjectPrefsGUI,
				keywords = KEYWORDS
			};
		}

		/// <summary>
		/// Opens the window for the Genesis Project Settings.
		/// </summary>
		public static void OpenProjectSettings()
		{
			SettingsService.OpenProjectSettings(PROJECT_TITLE_PATH);
		}

		private static void DrawPersonalPrefsGUI(string value = "")
		{
			EditorGUILayout.LabelField(USER_PREFERENCES_HEADER, EditorStyles.boldLabel);

			// Enable Dry Run
			EditorGUILayout.HelpBox(ENABLE_DRY_RUN_DESCRIPTION, MessageType.Info);
			using(var scope = new EditorGUI.ChangeCheckScope())
			{
				var newValue = EditorGUILayout.Toggle(ENABLE_DRY_RUN_LABEL, ExecuteDryRun);
				if(scope.changed)
				{
					ExecuteDryRun = newValue;
				}
			}

			// Enable Verbose Logging
			EditorGUILayout.HelpBox(ENABLE_VERBOSE_LOGGING_DESCRIPTION, MessageType.Info);
			using (var scope = new EditorGUI.ChangeCheckScope())
			{
				var newValue = EditorGUILayout.Toggle(ENABLE_VERBOSE_LOGGING_LABEL, EnableVerboseLogging);
				if (scope.changed)
				{
					EnableVerboseLogging = newValue;
				}
			}
		}

		private static void DrawProjectPrefsGUI(string value = "")
		{
			EditorGUILayout.LabelField(PROJECT_REFERENCES_HEADER, EditorStyles.boldLabel);

			// Genesis Installation Path
			EditorGUILayout.HelpBox(GENESIS_CLI_DIRECTORY_DESCRIPTION, MessageType.Info);

			// Show warnings if needed for any installation path issues.
			var currentFolder = GenesisCLIInstallationFolder;
			if (!Directory.Exists(currentFolder))
			{
				EditorGUILayout.HelpBox(DIRECTORY_DOES_NOT_EXIST_WARNING, MessageType.Error);
			}
			else if (!File.Exists(GetExecutablePath()))
			{
				EditorGUILayout.HelpBox(GENESIS_EXE_DOES_NOT_EXIST_WARNING, MessageType.Error);
			}

			// Draw selector for Installation Path
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(GENESIS_CLI_INSTALL_FOLDER_LABEL, GUILayout.MaxWidth(110f));
				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					var newValue = EditorGUILayout.TextField(GenesisCLIInstallationFolder);
					if (scope.changed)
					{
						GenesisCLIInstallationFolder = newValue;
						ProjectSettings.Save();
					}
				}

				if (GUILayoutTools.DrawFolderPickerLayout(ref currentFolder, GENESIS_CLI_FOLDER_SELECT_TITLE))
				{
					GenesisCLIInstallationFolder = currentFolder;
				}
			}

			// Update Genesis CLI button
			using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(GenesisCLIInstallationFolder) ||
			                                   !Directory.Exists(GenesisCLIInstallationFolder)))
			{
				const string UPDATE_GENESIS_CLI = "Update Genesis CLI";
				if (GUILayout.Button(UPDATE_GENESIS_CLI))
				{
					if (AutoUpdateDetector.TryUpdateGenesisCLI(autoUpdateWithoutPrompt: true))
					{
						Debug.Log(EditorConstants.GENESIS_IS_UP_TO_DATE);
					}
					else
					{
						Debug.LogWarning(EditorConstants.GENESIS_FAILED_TO_UPDATE);
					}
				}
			}

			// Plugins Section
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(PLUGIN_SETTINGS_HEADER, EditorStyles.boldLabel);

			const float PLUGINS_WIDTH = 240f;

			EditorGUILayout.HelpBox(EditorConstants.LOAD_UNSAFE_PLUGINS_DESCRIPTION, MessageType.Info);
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(EditorConstants.LOAD_UNSAFE_PLUGINS_TOGGLE_TEXT, GUILayout.MaxWidth(PLUGINS_WIDTH));
				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					var newValue = EditorGUILayout.Toggle(LoadUnsafePlugins);
					if (scope.changed)
					{
						LoadUnsafePlugins = newValue;
						ProjectSettings.Save();
					}
				}
			}

			EditorGUILayout.Space();

			EditorGUILayout.HelpBox(EditorConstants.LOAD_CURRENT_DOMAIN_ASSEMBLIES_DESCRIPTION, MessageType.Info);
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(EditorConstants.LOAD_CURRENT_DOMAIN_ASSEMBLIES_TOGGLE_TEXT, GUILayout.MaxWidth(PLUGINS_WIDTH));
				using (var scope = new EditorGUI.ChangeCheckScope())
				{
					var newValue = EditorGUILayout.Toggle(LoadCurrentDomainAssemblies);
					if (scope.changed)
					{
						LoadCurrentDomainAssemblies = newValue;
						ProjectSettings.Save();
					}
				}
			}

			// Plugin Installers Section
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(PLUGIN_INSTALLERS_HEADER, EditorStyles.boldLabel);
			EditorGUILayout.HelpBox(EditorConstants.INSTALL_ALL_PLUGINS_BUTTON_DESCRIPTION, MessageType.Info);

			var pluginInstallerConfigs =
				AssetDatabaseTools.GetAssets<PluginInstallerConfig>()
					.OrderBy(x => x.DisplayName);

			var anyPluginInstallers = !pluginInstallerConfigs.Any();

			// Install all plugins button
			using (new EditorGUI.DisabledScope(anyPluginInstallers))
			{
				if (GUILayout.Button(EditorConstants.INSTALL_ALL_PLUGINS_BUTTON_TEXT))
				{
					foreach (var pluginInstallerConfig in pluginInstallerConfigs)
					{
						if (!pluginInstallerConfig.CanAttemptPluginInstall)
						{
							continue;
						}

						pluginInstallerConfig.InstallPlugins();
					}
				}
			}

			// If there are none, show label that there are none
			if (anyPluginInstallers)
			{
				EditorGUILayout.LabelField(NONE_FOUND_DESCRIPTION);
			}
			// Otherwise show elements for all plugin installers
			else
			{
				foreach (var pluginInstallerConfig in pluginInstallerConfigs)
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						const string DISPLAY_NAME_PREFIX = "Name: ";

						EditorGUILayout.LabelField(
							DISPLAY_NAME_PREFIX,
							EditorStyles.boldLabel,
							GUILayout.Width(50f));
						EditorGUILayout.LabelField(pluginInstallerConfig.DisplayName);
						GUILayout.FlexibleSpace();

						using (new EditorGUI.DisabledScope(!pluginInstallerConfig.CanAttemptPluginInstall))
						{
							if (GUILayout.Button(EditorConstants.INSTALL_PLUGIN_BUTTON_TEXT))
							{
								pluginInstallerConfig.InstallPlugins();
							}
						}
					}

					var rect = GUILayoutUtility.GetLastRect();
					GUI.Box(rect, string.Empty);
				}
			}
		}

		#endregion

		#region Command-line

		/// <summary>
		/// Returns the absolute path to the <see cref="EditorConstants.GENESIS_EXECUTABLE"/> executable.
		/// </summary>
		public static string GetExecutablePath()
		{
			if (string.IsNullOrEmpty(GenesisCLIInstallationFolder))
			{
				return string.Empty;
			}
			else
			{
				return Path.Combine(Path.GetFullPath(GenesisCLIInstallationFolder), EditorConstants.GENESIS_EXECUTABLE);
			}
		}

        /// <summary>
        /// Returns the absolute path to the <see cref="EditorConstants.ASSEMBLY_LIST_FILE_PATH"/> executable.
        /// </summary>
        public static string GetAssemblyListFilePath()
        {
            if (string.IsNullOrEmpty(GenesisCLIInstallationFolder))
            {
                return EditorConstants.ASSEMBLY_LIST_FILE_PATH;
            }
            else
            {
                return Path.Combine(Path.GetFullPath(GenesisCLIInstallationFolder), EditorConstants.ASSEMBLY_LIST_FILE_PATH);
            }
        }

        /// <summary>
        /// Returns the absolute path template to the <see cref="EditorConstants.CONFIG_FILE_PATH_TEMPLATE"/> executable.
        /// </summary>
        public static string GetConfigFilePathTemplate()
        {
            if (string.IsNullOrEmpty(GenesisCLIInstallationFolder))
            {
                return EditorConstants.CONFIG_FILE_PATH_TEMPLATE;
            }
            else
            {
                return Path.Combine(Path.GetFullPath(GenesisCLIInstallationFolder), EditorConstants.CONFIG_FILE_PATH_TEMPLATE);
            }
        }

        /// <summary>
        /// Returns the absolute path to the working folder the the <see cref="EditorConstants.GENESIS_EXECUTABLE"/>.
        /// </summary>
        public static string GetWorkingPath()
		{
			return !string.IsNullOrEmpty(GenesisCLIInstallationFolder)
				? Path.GetFullPath(GenesisCLIInstallationFolder)
				: string.Empty;
		}

		#endregion
	}
}
