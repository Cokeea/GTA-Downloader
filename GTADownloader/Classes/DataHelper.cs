﻿using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GTADownloader
{
    class DataHelper
    {
        private static MainWindow Win = (MainWindow)Application.Current.MainWindow;

        public static DriveService service = new DriveService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyB8KixGHl2SPwQ5HJixKGm7IGbOYbpuc1w"
        });
        public static void EnableTaskBar()
        {
            Data.notifyIcon.Visible = true;
            Data.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            Data.notifyIcon.Text = "GTA Mission Downloader";
            Data.notifyIcon.Click += (sender, e) => NotifyIconBalloonTipClicked();
        }
        public static async void NotifyIconBalloonTipClicked(bool stopOnStart = true, bool updateFiles = true)
        {
            if (stopOnStart) StopNotification();
            Win.WindowState = WindowState.Normal;
            Win.Show();
            if (updateFiles) await CheckForUpdate.UpdateAsync(Data.ctsOnStart.Token);
        }
        public static FilesResource.GetRequest GetFileRequest(string fileID, string field)
        {
            var request = service.Files.Get(fileID);
            request.Fields = $"{field}";

            return request;
        }
        public static async Task PopulateFileNameArray()
        {
            int i = 0;
            foreach (var item in Data.fileIDArray)
            {
                var request = await GetFileRequest(item, "name").ExecuteAsync();
                Data.fileNameArray[i++] = request.Name;
            }
        }
        public static void StopNotification()
        {
            Data.ctsOnStart.Cancel();
            Data.ctsOnStart.Dispose();
            Data.ctsOnStart = new CancellationTokenSource();

            Win.TextTopOperationNotice.Text = "";
            Win.TextTopOperationProgramNotice.Text = "";

            Win.ProgramUpdateName.Visibility = Visibility.Hidden;
            Win.ReadChangelogName.Visibility = Visibility.Hidden;
        }
        public static void ButtonsOption(string whichOption)
        {
            switch (whichOption)
            {
                case "beforeDownload":
                    Win.progressBarDownload.Visibility = Visibility.Visible;
                    Win.textblockDownload.Visibility = Visibility.Visible;

                    Win.S1AltisButton.IsEnabled = false;
                    Win.S2AltisButton.IsEnabled = false;

                    Win.S3MaldenButton.IsEnabled = false;
                    Win.S3TanoaButton.IsEnabled = false;

                    Win.S1S2AltisButton.IsEnabled = false;
                    Win.S3MaldenS3TanoaButton.IsEnabled = false;
                    Win.AllFiles.IsEnabled = false;

                    Win.StopDownloadName.Visibility = Visibility.Visible;
                    break;
                case "afterDownload":
                    Win.progressBarDownload.Visibility = Visibility.Hidden;
                    Win.textblockDownload.Visibility = Visibility.Hidden;

                    Win.S1AltisButton.IsEnabled = true;
                    Win.S2AltisButton.IsEnabled = true;

                    Win.S3MaldenButton.IsEnabled = true;
                    Win.S3TanoaButton.IsEnabled = true;

                    Win.S1S2AltisButton.IsEnabled = true;
                    Win.S3MaldenS3TanoaButton.IsEnabled = true;
                    Win.AllFiles.IsEnabled = true;

                    Win.textblockDownload.Text = "";
                    Win.progressBarDownload.Value = 0;
                    Win.StopDownloadName.Visibility = Visibility.Hidden;
                    break;
                case "optionsCheckBoxOff":
                    Win.S1AltisCheckBox.IsChecked = false;
                    Win.S2AltisCheckBox.IsChecked = false;
                    Win.S3MaldenCheckBox.IsChecked = false;
                    Win.S3TanoaCheckBox.IsChecked = false;
                    break;
                case "deleteChangesToRegistry":
                    Win.StartUpCheckBox.IsChecked = false;
                    Win.HiddenCheckBox.IsChecked = false;
                    Win.RunTSAutoCheckBox.IsChecked = false;
                    Win.NotificationCheckBox.IsChecked = false;
                    Win.AutomaticUpdateCheckBox.IsChecked = false;
                    break;
            }
        }
    }
}