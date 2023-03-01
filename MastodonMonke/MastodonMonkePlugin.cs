using BepInEx;
using Bepinject;

namespace MastodonMonke
{
    [BepInPlugin("org.kaylie.gorillatag.mastodonmonke", "MastodonMonke", "1.1.2")]
    public class MastodonMonkePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Zenjector.Install<MainInstaller>().OnProject();

        }
    }
}
