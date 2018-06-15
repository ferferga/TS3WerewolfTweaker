using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace WerewolfTweaker.Effects.Helpers
{
    public class Uninstall
    {
        internal static bool kInstantiator = false;
        public static CommandHandler UninstallWerewolfTweaker = new CommandHandler(cleanWerewolfTweakerThings);
        public static CommandHandler HelpInUninstallWerewolfTweaker = new CommandHandler(ShowUninstallHelp);


        static Uninstall()
        {
            CommandSystem.RegisterCommand("WerewolfTweaker_Uninstall", "Run before uninstalling Werewolf Tweaker. Removes all added things by the mod to complete the uninstalling (OBJECTS AND BUFFS AREN´T REMOVED.IT DOESN´T HAVE SPECIAL REQUIRIMENTS IN UNINSTALLING).To complete the uninstall, simply remove the .package file from the Mods folder. Also is highly recommended to clean all cache files and reset the city with MasterController. If you need help, run this command:'WerewolfTweaker_UninstallHelp'. -fer456", Uninstall.UninstallWerewolfTweaker);
            CommandSystem.RegisterCommand("WerewolfTweaker_UninstallHelp", "Run it if you don´t know how to uninstall the mod. It will show up a little tutorial with explanations.", Uninstall.HelpInUninstallWerewolfTweaker);
        }
        public static int cleanWerewolfTweakerThings(object[] args)
        {
            try
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Started the Uninstall process...\nDuring the process, the mod will generate messages like this format and notifications for make more easy to track the uninstall errors. If you get notifications, the process is failed in a point, but if you don´t get notifications, the uninstall process was completed succesfully without problems. Also, when occurs an error, messages like this will be created. Example of text:\n'An error is ocurred when uninstalling. Check notification bar'.\nMORE HELP:RUN THE COMMAND 'WerewolfTweaker_UninstallHelp");
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Cleaning trackers...");
                CleanTrackers();
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Cleaned trackers succesfully.\nStarting the removal process of the makeups and dictionary...");
                CleanMakeup();                
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Finished the uninstall process.\nPlease wait to a newer message before saving.");
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "DO NOT FORGET TO CHECK THE NOTIFICATIONS ZONE TO CHECK THE UNINSTALL PROBLEMS. IN NORMAL CASES, YOU MUSTN´T HAVE NOTIFICATIONS CREATED BY THE MOD.\nIf there isn´t messages in the notification bar, you make the uninstall correctly. That´s good thing :)");
                AlarmManager.Global.AddAlarm(15f, TimeUnit.Minutes, new AlarmTimerCallback(Uninstall.ReceiveFinalMessage), "", AlarmType.NeverPersisted, null);
            }
            catch (Exception exception)
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "An error is ocurred when uninstalling. Check notification bar");
                StyledNotification.Show(new StyledNotification.Format("The main process of the uninstall is failed. Try again the process. Retry the uninstall. If you get the same error, run the command WerewolfTweaker_UninstallHelp to get more information \n" + exception, StyledNotification.NotificationStyle.kDebugAlert));
            }
            {
                return 1;
            }
        }

        private static void ReceiveFinalMessage()
        {
            SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "Finished the uninstall! Save your game and remove the package.");
        }
        public static int CleanTrackers()
        {
            try
            {                
                EventTracker.RemoveListener(WolfFeet.sWolfFeetBuffListener);
                EventTracker.RemoveListener(SkinAdd.sSkinAddBuffListener);
            }
            catch (Exception exception)
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "An error is ocurred when uninstalling. Check notification bar");
                StyledNotification.Show(new StyledNotification.Format("Failed the uninstall in the removal of trackers. Retry the uninstall. If you get the same error, run the command WerewolfTweaker_UninstallHelp to get more information \n" + exception, StyledNotification.NotificationStyle.kDebugAlert));
            }
            return 1;
        }
        public static int CleanMakeup()
        {
            try
            {               
                WolfFeet.cleanWerewolfFeets(true);
                SkinAdd.cleanWereSkinAdds(true);
            }
            catch (Exception exception)
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Uninstalling Wizard", "An error is ocurred when uninstalling. Check notification bar");
                StyledNotification.Show(new StyledNotification.Format("Failed the uninstall process in the removal of the makeup. Retry the uninstall. If you get the same error, run the command WerewolfTweaker_UninstallHelp to get more information \n" + exception, StyledNotification.NotificationStyle.kDebugAlert));
            }
            return 1;
        }
        public static int ShowUninstallHelp(object[] args)
        {
            try
            {
                SimpleMessageDialog.Show("Werewolf Tweaker - Help Step 2", "When finish the uninstall, you will receive a confirmation message");
                SimpleMessageDialog.Show("Werewolf Tweaker - Help Step 1", "When you run the 'WerewolfTweaker_Uninstall' command, the script will start the process of the uninstall of the mod. Windows like this will show up information in real time during the uninstall process.\nThis type of Window also make some advices and show up a message each time that something happens.\nIt´s a easy method to know the state of the removal.");
                StyledNotification.Show(new StyledNotification.Format("Notification like this is shown when an error occurs. It is strange that happens an error, but it can always occur. It will show a small message with information and a summary of code to help understand the error. Don't worry if you don't see a notification, that means that everything went well and you can finally uninstall the mod", StyledNotification.NotificationStyle.kDebugAlert));
                StyledNotification.Show(new StyledNotification.Format("If you run this command because you have problems with the uninstall, retry the process. If it still with errors, reset the city with MasterController and do again the process. If still with problems, copy the error code and post a comment or send a message (to fer456) in the Mod thread's. Make sure you copy without errors the entire error code.", StyledNotification.NotificationStyle.kSystemMessage));
            }
            catch
            {
            }
            return 1;
        }
    }
}
