using Buttr.Injection;
using UnityEngine;

namespace Buttr.Tests.Editor.Unity.Injection {
    public partial class InjectableSettings : ScriptableObject {
        [Inject] public IClock Clock;
    }

    public interface IClock { }

    public sealed class SystemClock : IClock { }
}
