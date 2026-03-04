using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace AutoArmorUpgrade {
    public class MySubModule : MBSubModuleBase {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
            base.OnGameStart(game, gameStarterObject);

            if (gameStarterObject is CampaignGameStarter campaignGameStarter) {
                campaignGameStarter.AddBehavior(new AutoArmorBehavior());
            }
        }


        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
        }
    }
}
