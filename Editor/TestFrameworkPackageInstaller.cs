using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace CeejayZSmith.TestRunnerSetup.Editor
{
    public static class TestFrameworkPackageInstaller
    {
        private const string TestPackageName = "com.unity.test-framework";

        [UsedImplicitly]
        public static async void InstallLatestAndQuit()
        {
            try
            {
                await InstallLatest();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(-1);
            }
            
            EditorApplication.Exit(0);
        }

        [UsedImplicitly]
        public static async Task InstallLatest()
        {
            bool result = await EnsureTestFrameworkPackageIsInstalled();

            if (result == false)
            {
                throw new Exception($"Failed to install '{TestPackageName}'");
            }

            Debug.Log($"{TestPackageName} is installed.");
        }

        private static async Task<bool> EnsureTestFrameworkPackageIsInstalled()
        {
            if (await IsPackageInstalled(TestPackageName) == true)
            {
                Debug.Log($"{TestPackageName} is already installed.");
                return true;
            }

            return await InstallPackage(TestPackageName);
        }

        private static async Task<bool> IsPackageInstalled(string packageName)
        {
            Debug.Log($"Checking installed packages for {packageName}...");
            
            ListRequest listRequest = Client.List(); 
            while (!listRequest.IsCompleted)
                await Task.Yield();

            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    if (package.name == packageName)
                        return true;
                }
            }
            
            return false;
        }
    
        private static async Task<bool> InstallPackage(string packageName)
        {
            Debug.Log($"Installing {packageName}...");
            
            AddRequest addRequest = Client.Add(packageName); 
            while (!addRequest.IsCompleted)
                await Task.Yield();

            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log($"{packageName} installed successfully.");
                return true;
            }
            
            Debug.LogError($"Failed to install {packageName}: {addRequest.Error.message}");
            return false;
        }
    }
}

