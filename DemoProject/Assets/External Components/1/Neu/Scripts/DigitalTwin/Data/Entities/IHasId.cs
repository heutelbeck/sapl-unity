namespace FTK.SturmMixedReality.Data.Entities {
    public interface IHasId {
        string Id { get; }
    }

    public interface IHasShortId : IHasId {
        string ShortId { get; }
    }
}