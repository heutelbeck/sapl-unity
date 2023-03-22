using FTK.SturmMixedReality.Data.Serialization;

namespace FTK.SturmMixedReality.Data.Entities {
    public class Printer : JSONSerializable<Printer>, IJSONSerializable, IHasShortId {
        public string Id { get; set; }
        public string ShortId { get; set; }
        public string Name { get; set; }
        public BuildUnit BuildUnit { get; set; }
        public float RemainingPrintintTime { get; set; }
    }
}
