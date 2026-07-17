namespace Buttr.Core {
    /// <summary>
    /// Example Entity. 
    /// </summary>
    public sealed class Entity : IEntity<Identifier> {
        public Identifier ID { get; } 
    }
}