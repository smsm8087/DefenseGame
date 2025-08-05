using System;
using DataModels;
using UnityEngine;
using UI;
using System.Collections;

public class NoticePopupHandler : INetworkMessageHandler
{
    public string Type => "notice";
    private IEnumerator Show(NetMsg msg)
    {
        yield return null;
        Action createAndInit = () =>
        {
            var popup = PopupManager.Instance.ShowPopup<NoticePopup>(
                PopupManager.Instance.noticePrefab
            );
            if (popup == null)
                return;  // 로딩 중이라면 일단 무시

            Debug.Log($"[Debug–Client] ShowPopup<NoticePopup> 반환 확인 → popup is null? {popup == null}");

            popup.Init(
                msg.message,
                onOk: () => { }
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