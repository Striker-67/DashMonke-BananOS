using System;
using UnityEngine.XR;
using UnityEngine;
using BananaOS;
using System.Text;
using BananaOS.Pages;
using System.Collections;
using Photon.Pun;
using GorillaNetworking;

namespace DashMonkebananaos
{
    public class DashManager : WatchPage
    {

        public bool Dashing { get; private set; }
        public bool CanDash { get; private set; }
        public Vector3 Direction { get; private set; }
        float power = 300f;
        float cooldown = 1f;
        Moddedcheck moddedcheck;
        public override string Title => "DashMonke";

        public override bool DisplayOnMainMenu => true;

        public void Start()
        {
            gameObject.AddComponent<Moddedcheck>();
            moddedcheck = FindAnyObjectByType<Moddedcheck>();
        }
        public override void OnPostModSetup()
        {
            //max selection index so the indicator stays on the screen
            selectionHandler.maxIndex = 3;
        }
        public override string OnGetScreenContent()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DashMonke");
            if (PhotonNetwork.InRoom && moddedcheck.IsModded())
            {
                if (CanDash)
                {
                    stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Enabled"));
                }
                else
                {
                    stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(0, "Disabled"));
                }

                stringBuilder.AppendLine("<color=red>==</color> settings <color=red>==</color>");
                stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(1, "Power: " + power));
                stringBuilder.AppendLine("<color=red>setting to low may cause you not to move</color>");
                stringBuilder.AppendLine(selectionHandler.GetOriginalBananaOSSelectionText(2, "CoolDown: " + cooldown));
                BananaNotifications.DisplayNotification("Your in a modded", Color.yellow, Color.white, 1);

            }
            else
            {
                BananaNotifications.DisplayNotification("Your NOT in a modded", Color.red, Color.white, 1);
                stringBuilder.AppendLine("you're either not in a room\n\nor you are not in a modded");
            }
             return stringBuilder.ToString();
        }
        public override void OnButtonPressed(WatchButtonType buttonType)
        {

            switch (buttonType)
            {
                case WatchButtonType.Up:
                    selectionHandler.MoveSelectionUp();
                    break;

                case WatchButtonType.Down:
                    selectionHandler.MoveSelectionDown();
                    break;

                case WatchButtonType.Left:
                    if(selectionHandler.currentIndex == 1)
                    {
                        if(power > 100)
                        {
                            power -= 5f;
                        }
                       
                    }
                    if (selectionHandler.currentIndex == 2)
                    {
                        if(cooldown > 0)
                        {
                            cooldown--;
                        }
                    }
                    break;
                case WatchButtonType.Right:
                    if (selectionHandler.currentIndex == 1)
                    {
                        power+= 5f;
                    }
                    if (selectionHandler.currentIndex == 2)
                    {
                        cooldown++;
                    }
                    break;
                case WatchButtonType.Enter:
                    if (selectionHandler.currentIndex == 0)
                    {
                        if (!CanDash)
                        {
                            CanDash = true;
                        }
                        else
                        {
                            CanDash = false;
                        }

                        return;
                    }
                    break;

                //It is recommended that you keep this unless you're nesting pages if so you should use the SwitchToPage method
                case WatchButtonType.Back:
                    ReturnToMainMenu();
                    break;
            }
        }
        public void Update()
        {
            if (CanDash && ControllerInputPoller.instance.leftControllerPrimaryButton)
            {

                    GorillaTagger.Instance.rigidbody.AddForce(GorillaTagger.Instance.headCollider.transform.forward * power, ForceMode.Impulse);
                StartCoroutine("Cooldown");


            }
            if (PhotonNetwork.InRoom && !moddedcheck.IsModded())
            {
                CanDash = false;
            }
            if(!PhotonNetwork.InRoom)
            {
                CanDash = false;
            }
            

        }
        IEnumerator Cooldown()
        {
            CanDash = false;
            yield return new WaitForSeconds(cooldown);
            CanDash = true;
        }
        

    }
       
}
public class Moddedcheck : MonoBehaviourPunCallbacks
{
    public static Moddedcheck modcheck;
    public object gameMode;

public bool IsModded()
{
    if (!PhotonNetwork.InRoom)
        return false;
    return gameMode.ToString().Contains("MODDED");
}

    public override void OnJoinedRoom()
    {
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out gameMode);
        BananaOS.MonkeWatch.Instance.UpdateScreen();
    }
    public override void OnLeftRoom()
    {
        BananaOS.MonkeWatch.Instance.UpdateScreen();
    }
}