using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UADAPI
{
    public enum EpisodeDownloadState
    {
        NotDownloaded, InDownloadQueue, FinishedDownloading, FailedDownloading, Downloading
    }

    public enum UADDownloaderState
    {
        NotStarted, Working, Paused, Finished, Canceled
    }
}
