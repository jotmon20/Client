﻿using SearchWebsite;
using System;
using System.Globalization;
using System.Threading;
using TorrentClient;
using TorrentClient.Extensions;


namespace TorrentClientApp
{
    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private static TorrentClient.TorrentClient torrentClient;

        /// <summary>
        /// The torrent.
        /// </summary>
        private static TorrentInfo torrent;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            string Torrent_path = Generate_torrent.generate_torrent_F();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            

            TorrentInfo.TryLoad(Torrent_path, out torrent);
            

            torrentClient = new TorrentClient.TorrentClient("Enter port you want to use", @"Enter where your torrent file is saved"); 
            torrentClient.DownloadSpeedLimit = 1000000 * 1024; // 100 KB/s
            torrentClient.UploadSpeedLimit = 2000000 * 1024; // 200 KB/s
            torrentClient.TorrentHashing += TorrentClient_TorrentHashing;
            torrentClient.TorrentLeeching += TorrentClient_TorrentLeeching;
            torrentClient.TorrentSeeding += TorrentClient_TorrentSeeding;
            torrentClient.TorrentStarted += TorrentClient_TorrentStarted;
            torrentClient.TorrentStopped += TorrentClient_TorrentStopped;
            torrentClient.Start(); // start torrent client
            torrentClient.Start(torrent); // start torrent file

            // setup checkout timer
            var timer = new System.Timers.Timer();
            timer.Interval = 1;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();

            Thread.Sleep(10000000);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.Error.WriteLine("Unhandeled exception occured:");
            Console.Error.WriteLine((e.ExceptionObject as Exception).StackTrace);
        }

        /// <summary>
        /// Handles the Elapsed event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TorrentProgressInfo info = torrentClient.GetProgressInfo(torrent.InfoHash);

            Console.WriteLine(string.Empty);
            Console.WriteLine($"\tduration: {info.Duration.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)}\tcompleted: {(int)Math.Round(info.CompletedPercentage * 100)}%");
            Console.WriteLine($"\tdownload speed: {info.DownloadSpeed.ToBytes()}/s\tupload speed: {info.UploadSpeed.ToBytes()}/s");
            Console.WriteLine($"\tdownloaded: {info.Downloaded.ToBytes()}\tuploaded: {info.Uploaded.ToBytes()}");
            Console.WriteLine($"\tseeders: {info.SeederCount}\t\tleechers: {info.LeecherCount}");
            
        }

        /// <summary>
        /// Handles the TorrentHashing event of the TorrentClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TorrentHashingEventArgs"/> instance containing the event data.</param>
        private static void TorrentClient_TorrentHashing(object sender, TorrentHashingEventArgs e)
        {
            Console.WriteLine("hashing");
        }

        /// <summary>
        /// Handles the TorrentLeeching event of the TorrentClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TorrentLeechingEventArgs"/> instance containing the event data.</param>
        private static void TorrentClient_TorrentLeeching(object sender, TorrentLeechingEventArgs e)
        {
        }

        /// <summary>
        /// Handles the TorrentSeeding event of the TorrentClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TorrentSeedingEventArgs"/> instance containing the event data.</param>
        private static void TorrentClient_TorrentSeeding(object sender, TorrentSeedingEventArgs e)
        {
            Console.WriteLine("seeding");
        }

        /// <summary>
        /// Handles the TorrentStarted event of the TorrentClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TorrentStartedEventArgs"/> instance containing the event data.</param>
        private static void TorrentClient_TorrentStarted(object sender, TorrentStartedEventArgs e)
        {
            Console.WriteLine("started");
        }

        /// <summary>
        /// Handles the TorrentStopped event of the TorrentClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TorrentStoppedEventArgs"/> instance containing the event data.</param>
        private static void TorrentClient_TorrentStopped(object sender, TorrentStoppedEventArgs e)
        {
            Console.WriteLine("stopped");
        }

        #endregion Private Methods
    }
}
