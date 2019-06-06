using System;
using System.Collections.Generic;
using System.Text;

namespace SegmentDownloader.Core
{
    [Serializable]
    public struct CalculatedSegment
    {
        private long startPosition;
        private long endPosition;

        public long StartPosition
        {
            get { return startPosition; }
        }

        public long EndPosition
        {
            get { return endPosition; }
        }

        public CalculatedSegment(long startPos, long endPos)
        {
            this.endPosition = endPos;
            this.startPosition = startPos;
        }
    }
}
