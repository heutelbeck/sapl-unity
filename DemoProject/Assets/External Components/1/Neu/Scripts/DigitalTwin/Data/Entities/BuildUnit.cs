using FTK.SturmMixedReality.Data.Serialization;

namespace FTK.SturmMixedReality.Data.Entities {
    public class BuildUnit : JSONSerializable<BuildUnit>, IJSONSerializable, IHasShortId {
        public string Id { get; set; }
        public string ShortId { get; set; }
        public string Name { get; set; }
        public string Material { get; set; }
        public PrintJob PrintJob { get; set; }
    }
}