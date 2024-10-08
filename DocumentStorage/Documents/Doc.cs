using System;

namespace DocumentStorage.Documents
{
    class Doc 
    {
        private string title = string.Empty;
        private string fileName = string.Empty;
        private string comment = string.Empty;

        public long Id { get; set; }
        public string Title { get => title; set => title = value ?? string.Empty; }
        public string FileName { get => fileName; set => fileName = value ?? string.Empty; }
        public string Comment { get => comment; set => comment = value ?? string.Empty; }
        public byte[] DocData { get; set; }
        public byte[] DocSampleData { get; set; }
        public int State { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        public Doc()
        {
            State = 0;
        }
    }
}
