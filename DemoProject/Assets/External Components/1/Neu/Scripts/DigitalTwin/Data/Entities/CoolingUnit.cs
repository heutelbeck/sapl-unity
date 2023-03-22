using FTK.SturmMixedReality.Data.Serialization;

namespace FTK.SturmMixedReality.Data.Entities {
    public class CoolingUnit : JSONSerializable<CoolingUnit>, IJSONSerializable, IHasShortId {
        public string Id { get; set; }
        public string ShortId { get; set; }
        public string Name { get; set; }
        public PrintJob PrintJob { get; set; }
        public float RemainingCoolingTime { get; set; }
    }
}