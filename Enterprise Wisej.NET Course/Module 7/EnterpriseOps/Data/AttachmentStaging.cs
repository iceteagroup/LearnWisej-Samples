using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Where uploads wait between the Attachments step and Finish. Staged files are not part of the
    /// escalation until the workflow persists it; a cancelled wizard discards them (matrix: cancel after upload).
    /// </summary>
    public class AttachmentStaging
    {
        private readonly ActivityTrace _trace;
        private readonly Dictionary<string, Attachment> _staged = new Dictionary<string, Attachment>();

        // The "files" the lab picks from — no real upload, the point is the staging life-cycle.
        private static readonly (string Name, long Size)[] SampleFiles =
        {
            ("vendor_quote.pdf", 184_320),
            ("pump_photo.jpg", 2_621_440),
            ("sla_extract.xlsx", 61_440),
            ("site_report.docx", 122_880),
        };
        private int _next;

        public AttachmentStaging(ActivityTrace trace)
        {
            _trace = trace;
        }

        public int StagedCount => _staged.Count;

        /// <summary>Stages the next sample file and returns it.</summary>
        public Attachment StageNextSample()
        {
            var sample = SampleFiles[_next++ % SampleFiles.Length];
            var attachment = new Attachment
            {
                StagedId = Guid.NewGuid().ToString("N").Substring(0, 8),
                FileName = sample.Name,
                SizeBytes = sample.Size,
            };
            _staged[attachment.StagedId] = attachment;
            _trace.Write($"Data: staged {attachment.FileName} as {attachment.StagedId} ({_staged.Count} staged)");
            return attachment;
        }

        public void Discard(string stagedId)
        {
            if (_staged.Remove(stagedId))
                _trace.Write($"Data: discarded staged {stagedId} ({_staged.Count} staged)");
        }

        /// <summary>Cancel path: every staged upload of this draft is removed.</summary>
        public int DiscardAll(IEnumerable<Attachment> attachments)
        {
            int count = 0;
            foreach (var a in attachments.ToList())
            {
                if (_staged.Remove(a.StagedId)) count++;
            }
            _trace.Write($"Data: staging cleaned up — {count} upload(s) removed");
            return count;
        }

        /// <summary>Persist path: the files become part of the escalation and leave staging.</summary>
        public void Commit(IEnumerable<Attachment> attachments)
        {
            int count = 0;
            foreach (var a in attachments)
            {
                if (_staged.Remove(a.StagedId)) count++;
            }
            _trace.Write($"Data: {count} attachment(s) committed from staging");
        }
    }
}
