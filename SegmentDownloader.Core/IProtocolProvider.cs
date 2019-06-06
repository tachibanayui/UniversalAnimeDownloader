using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SegmentDownloader.Core
{
    public interface IProtocolProvider
    {
        // TODO: remove this method? Acoplamento ficara s� de um lado
        void Initialize(Downloader downloader);

        Stream CreateStream(ResourceLocation rl, long initialPosition, long endPosition, WebHeaderCollection headers);

        RemoteFileInfo GetFileInfo(ResourceLocation rl, WebHeaderCollection headers, out Stream stream);
    }
}
