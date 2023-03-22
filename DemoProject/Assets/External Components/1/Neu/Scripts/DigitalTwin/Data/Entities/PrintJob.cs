using FTK.SturmMixedReality.Data.Serialization;

namespace FTK.SturmMixedReality.Data.Entities {
    public class PrintJob : JSONSerializable<PrintJob>, IJSONSerializable, IHasShortId {
        public string Id { get; set; }
        public string ShortId { get; set; }
        public string Name { get; set; }
        public string Customer { get; set; }
        public float Duration { get; set; }
        public bool IsPrinted { get; set; }
    }
}