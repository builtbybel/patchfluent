//
// Patchlady
// Copyright 2016 Vyacheslav Napadovsky.
// Forked as Patchlady, Builtbybel (C) 2020
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Patchlady
{

    public partial class MainWindow : Window
    {
        private dynamic _updateSession = null;
        private dynamic _updateSearcher = null;
        private dynamic _searchResult = null;

        private async Task SearchForUpdates()
        {
            _status.Text = "Checking for updates ...";
            await Task.Run(() =>
            {
                _searchResult = _updateSearcher.Search("IsInstalled=0");
            });
            _status.Text = "Updates are available.";
            var list = new List<UpdateItem>();
            int count = _searchResult.Updates.Count;
            _installButton.IsEnabled = count > 0;
            if (count > 0)
            {
                for (int i = 0; i < _searchResult.Updates.Count; ++i)
                    list.Add(new UpdateItem(_searchResult.Updates.Item(i)));
            }
            else
            {
                _status.Text = "Your device is up to date.";
            }
            _list.ItemsSource = list;
        }

        protected override async void OnActivated(EventArgs e) 
        {
            base.OnActivated(e);
            if (_updateSession == null)
            {
                try
                {
                    _updateSession = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.Session"));
                    _updateSession.ClientApplicationID = "Patchlady";
                    _updateSearcher = _updateSession.CreateUpdateSearcher();
                    await SearchForUpdates();
                    _installButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), "Exception has occured!");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // GUI options
            // This is using font icons predefined in the fonts of Segoe MDL2 Assets
            _assetHamburger.Content = "\ue700";    // Menu icon
            _assetRefresh.Content= "\uecc5";       // Update icon

        }

        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            _installButton.IsEnabled = false;

            try
            {
                var list = _list.ItemsSource as List<UpdateItem>;
                dynamic updatesToInstall = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.UpdateColl"));
                foreach (var item in list)
                {
                    if (!item.IsChecked)
                        continue;
                    if (!item.EulaAccepted)
                    {
                        if (MessageBox.Show(this, item.Update.EulaText, "Do you accept this license agreement?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            continue;
                        item.Update.AcceptEula();
                    }
                    updatesToInstall.Add(item.Update);
                }
                if (updatesToInstall.Count == 0)
                {
                    _status.Text = "No updates are available.";
                }
                else
                {
                    _status.Text = "Downloading updates ...";
                    dynamic downloader = _updateSession.CreateUpdateDownloader();
                    downloader.Updates = updatesToInstall;
                    await Task.Run(() => { downloader.Download(); });

                   // if (MessageBox.Show(this, "Installation ready. Continue?", "Notice", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    //{
                        _status.Text = "Installing updates ...";

                        dynamic installer = _updateSession.CreateUpdateInstaller();
                        installer.Updates = updatesToInstall;
                        dynamic installationResult = null;
                        await Task.Run(() => { installationResult = installer.Install(); });

                        var sb = new StringBuilder();
                        if (installationResult.RebootRequired == true)
                            sb.Append("[REBOOT REQUIRED] ");
                        sb.AppendFormat("Code: {0}\n", installationResult.ResultCode);
                        sb.Append("Listing of updates installed:\n");
                        for (int i = 0; i < updatesToInstall.Count; ++i)
                        {
                            sb.AppendFormat("{0} : {1}\n",
                                installationResult.GetUpdateResult(i).ResultCode,
                                updatesToInstall.Item(i).Title);
                        }
                        MessageBox.Show(this, sb.ToString(), "Installation Result");
                             _description.Document.Blocks.Clear();
                    //}
                    await SearchForUpdates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Exception has occured!");
            }

            _installButton.IsEnabled = true;
       }

        private void ListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (UpdateItem item in e.AddedItems)
            {
                _description.Document.Blocks.Clear();
                var p = new Paragraph();
                p.Inlines.Add(new Run(item.Description));
                p.EnableHyperlinks();
                _description.Document.Blocks.Add(p);
                break;
            }
        }


        private void _imageGitHub_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/builtbybel/patchlady");
        }

        private void _linkWUpdate_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("ms-settings:windowsupdate");
        }


        private async void _assetRefresh_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await SearchForUpdates();
        }
    }

    internal static class RichEditExtensions
    {
        public static void EnableHyperlinks(this Paragraph p)
        {
            string paragraphText = new TextRange(p.ContentStart, p.ContentEnd).Text;
            foreach (string word in paragraphText.Split(' ', '\n', '\t').ToList())
            {
                if (word.IndexOf("//") != -1 && Uri.IsWellFormedUriString(word, UriKind.Absolute))
                {
                    Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri)
                        uri = new Uri(@"http://" + word, UriKind.Absolute);
                    for (TextPointer position = p.ContentStart;
                        position != null;
                        position = position.GetNextContextPosition(LogicalDirection.Forward))
                    {
                        if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                        {
                            string textRun = position.GetTextInRun(LogicalDirection.Forward);
                            int indexInRun = textRun.IndexOf(word);
                            if (indexInRun >= 0)
                            {
                                TextPointer start = position.GetPositionAtOffset(indexInRun);
                                TextPointer end = start.GetPositionAtOffset(word.Length);
                                var link = new Hyperlink(start, end);
                                link.NavigateUri = uri;
                                link.RequestNavigate += (sender, args) => Process.Start(args.Uri.ToString());
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}

internal class UpdateItem
{
    private readonly dynamic _update;

    private int GetSizeOrder(decimal size)
    {
        int order;
        for (order = 0; size > 1024; ++order)
        {
            size /= 1024;
        }
        return order;
    }

    private string SizeToString(decimal size, int order, bool addsuffix)
    {
        string fmt;
        switch (order)
        {
            case 0: fmt = "{0} B"; break;
            case 1: fmt = "{0} KB"; size /= 1024; break;
            case 2: fmt = "{0} MB"; size /= 1024 * 1024; break;
            default: fmt = "{0} GB"; size /= 1024 * 1024 * 1024; break;
        }
        return addsuffix
            ? string.Format(fmt, (int)size)
            : ((int)size).ToString();
    }

    public UpdateItem(dynamic update)
    {
        _update = update;

        //IsChecked = _update.AutoSelectOnWebSites;     // this line will select them all
        IsChecked = _update.IsMandatory;

        var sb = new StringBuilder();
        if (EulaAccepted == false)
            sb.Append("[EULA NOT ACCEPTED] ");
        sb.AppendFormat("{0}\n", _update.Title);
        if (_update.Description != null)
            sb.AppendFormat("{0}\n", _update.Description);
        if (_update.MoreInfoUrls != null && _update.MoreInfoUrls.Count > 0)
        {
            sb.AppendFormat("More info:\n");
            for (int i = 0; i < _update.MoreInfoUrls.Count; ++i)
                sb.AppendFormat("{0}\n", _update.MoreInfoUrls.Item(i));
        }
        if (_update.EulaText != null)
            sb.AppendFormat("EULA TEXT:\n{0}\n\n", _update.EulaText);
        if (_update.ReleaseNotes != null)
            sb.AppendFormat("Release Notes:\n{0}\n\n", _update.ReleaseNotes);

        dynamic bundle = _update.BundledUpdates;
        if (bundle != null && bundle.Count > 0)
        {
            sb.AppendFormat("This update contains {0} packages:\n", bundle.Count);
            for (int i = 0; i < bundle.Count; ++i)
            {
                var item = new UpdateItem(bundle.Item(i));
                var desc = item.Description;
                desc = desc.Substring(0, desc.Length - 1);
                sb.AppendFormat("#{0}: {1}\n", i + 1, desc.Replace("\n", "\n * "));
            }
        }

        decimal minSize = _update.MinDownloadSize;
        decimal maxSize = _update.MaxDownloadSize;
        string sizeString;
        if (minSize == 0 || minSize == maxSize)
        {
            sizeString = SizeToString(maxSize, GetSizeOrder(maxSize), true);
        }
        else
        {
            int order = Math.Max(GetSizeOrder(minSize), GetSizeOrder(maxSize));
            sizeString = string.Format("{0} - {1}",
                SizeToString(minSize, order, false),
                SizeToString(maxSize, order, true)
            );
        }
        Title = string.Format("{0} ({1})", _update.Title, sizeString);

        Description = sb.ToString();
    }

    public bool IsChecked { get; set; }
    public string Title { get; }
    public string Description { get; }

    public dynamic Update { get { return _update; } }
    public bool EulaAccepted { get { return _update.EulaAccepted; } }

    public Brush Background
    {
        get
        {
            return Brushes.Transparent;
            //return IsHidden ? SystemColors.InactiveCaptionTextBrush : Brushes.Transparent;
        }
    }
}