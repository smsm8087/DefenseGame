using System;
using System.Collections;
using DataModels;
using UnityEngine;

public class ConfirmPopupHandler : MonoBehaviour, INetworkMessageHandler
{
    public string Type => "confirm";

    private IEnumerator Show(NetMsg msg)
    {
        yield return null;
        Action createAndInit = () =>
        {
            var popup = PopupManager.Instance.ShowPopup<ConfirmPopup>(
                PopupManager.Instance.confirmPrefab
            );
            if (popup == null)
                return;  // 로딩 중이라면 일단 무시

            popup.Init(
                msg.message,
                onYes: () =>
                {
                    NetworkManager.Instance.SendMsg(new NetMsg
                    {
                        type = "confirm_response",
                        requestId = msg.requestId,
                        approved = true
                    });
                },
                onNo: () =>
                {
                    NetworkManager.Instance.SendMsg(new NetMsg
                    {
                        type = "confirm_response",
                        requestId = msg.requestId,
                        approved = false
                    });
                }
            );
        };

        if (PopupManager.Instance.IsLoading)
            PopupManager.Instance.EnqueueDeferred(createAndInit);
        else
            createAndInit();
    }
    public void Handle(NetMsg msg)
    {
        MainThreadUtil.Run(Show(msg));
    }
}