using FTK.SturmMixedReality.Data.Serialization;

namespace FTK.SturmMixedReality.Data.Entities {
    public class ProcessingStation : JSONSerializable<ProcessingStation>, IJSONSerializable, IHasShortId {
        public string Id { get; set; }
        public string ShortId { get; set; }
        public string Name { get; set; }
        public BuildUnit BuildUnit { get; set; }
        public CoolingUnit CoolingUnit { get; set; }
        public float RemainingProcessingTime { get; set; }
    }
}