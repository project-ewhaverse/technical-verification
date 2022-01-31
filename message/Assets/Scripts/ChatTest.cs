using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using ExitGames.Client.Photon;

public class ChatTest : MonoBehaviour, IChatClientListener
{
	private ChatClient chatClient;
	private string userName;
	private string currentChannelName;

	public InputField inputField;

	public GameObject YellowArea, WhiteArea, DateArea;
	public RectTransform ContentRect;
	public Scrollbar scrollBar;
	AreaScript LastArea;

	
	// Use this for initialization
	void Start()
	{

		Application.runInBackground = true;

		userName = System.Environment.UserName;
		currentChannelName = "Channel 001";

		chatClient = new ChatClient(this);
		chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName));

		//AddLine(string.Format("연결시도", userName));
	}

	/*
	public void AddLine(string lineString)
	{
		outputText.text += lineString + "\r\n";
	}
	*/


	public void OnApplicationQuit()
	{
		if (chatClient != null)
		{
			chatClient.Disconnect();
		}
	}

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
	{
		if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
		{
			Debug.LogError(message);
		}
		else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
		{
			Debug.LogWarning(message);
		}
		else
		{
			Debug.Log(message);
		}
	}

	public void OnConnected()
	{
		//AddLine("서버에 연결되었습니다.");
		Debug.Log("서버에 연결되었습니다.");

		chatClient.Subscribe(new string[] { currentChannelName }, 0);
	}

	public void OnDisconnected()
	{
		//AddLine("서버에 연결이 끊어졌습니다.");
		Debug.Log("서버에 연결이 끊어졌습니다.");
	}

	public void OnChatStateChange(ChatState state)
	{
		Debug.Log("OnChatStateChange = " + state);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		//AddLine(string.Format("채널 입장 ({0})", string.Join(",", channels)));
		Debug.Log(string.Format("채널 입장 ({0})", string.Join(",", channels)));
	}

	public void OnUnsubscribed(string[] channels)
	{
		//AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));
		Debug.Log(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		for (int i = 0; i < messages.Length; i++)
		{
			//AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
			if(senders[i]==userName)
				chat(true, messages[i].ToString(), senders[i], null);
			else
				chat(false, messages[i].ToString(), senders[i], null);
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		Debug.Log("OnPrivateMessage : " + message);
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
	}

	void Update()
	{
		chatClient.Service();
	}
	public void OnUserSubscribed(string channel, string user)
	{
		throw new System.NotImplementedException();
	}

	public void OnUserUnsubscribed(string channel, string user)
	{
		throw new System.NotImplementedException();
	}


	public void OnEnterSend()
	{
		if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
		{
			this.SendChatMessage(this.inputField.text);
			this.inputField.text = "";
			this.inputField.Select();
			this.inputField.ActivateInputField();
		}
	}

	private void SendChatMessage(string inputLine)
	{
		if (string.IsNullOrEmpty(inputLine))
		{
			return;
		}

		this.chatClient.PublishMessage(currentChannelName, inputLine);
	}





	//chat(true, inputField.text, "seung", null);
	public void chat(bool isSend, string text, string user, Texture picture)
	{
		//if (text.Trim() == "") return; // 공백 문자열은 제외

		bool isBottom = scrollBar.value <= 0.0001f; // 현재 스크롤바가 맨 밑에 위치해있는지
		//print(text);

		//보내는 사람은 노랑, 받는 사람은 흰색영역을 만들어 텍스트 대입
		AreaScript Area = Instantiate(isSend ? YellowArea : WhiteArea).GetComponent<AreaScript>();
		Area.transform.SetParent(ContentRect.transform, false);
		Area.BoxRect.sizeDelta = new Vector2(180, Area.BoxRect.sizeDelta.y);
		Area.TextRect.GetComponent<Text>().text = text;
		Fit(Area.BoxRect);

		// 가로길이 설정 (max 너비에 비해 짧을 경우 대비)
		float X = Area.TextRect.sizeDelta.x + 21;
		float Y = Area.TextRect.sizeDelta.y;

		if (Y > 15) // 한 줄 초과
		{
			for (int i = 0; i < 200; i++)
			{
				Area.BoxRect.sizeDelta = new Vector2(X - i * 2, Area.BoxRect.sizeDelta.y);
				Fit(Area.BoxRect);

				if (Y != Area.TextRect.sizeDelta.y) { Area.BoxRect.sizeDelta = new Vector2(X - (i * 2) + 2, Y); break; }
			}
		}
		else Area.BoxRect.sizeDelta = new Vector2(X, Y); // 한 줄

		//메세지 옆 시간 대입
		DateTime t = DateTime.Now;
		Area.Time = t.ToString("yyyy-MM-dd-HH-mm");
		Area.User = user;

		int hour = t.Hour;

		if (hour == 0) hour = 12;
		else if (hour > 12) hour -= 12;
		Area.TimeText.text = (t.Hour < 12 ? "오전 " : "오후 ") + hour + ":" + t.ToString("mm");

		// 이전 시간과 같으면 시간꼬리 없애기
		bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.User == Area.User;
		if (isSame) LastArea.TimeText.text = "";
		Area.Tail.SetActive(!isSame);

		// - 받은 메세지의 경우 이름, 프로필 사진도 없애기
		if (!isSend)
		{
			//Area.UserImage.gameObject.SetActive(!isSame); - padding 설정때문에 수정중
			//Area.UserText.gameObject.SetActive(!isSame);
			Area.UserText.text = user;
		}

		// 날짜 박스: 이전 것과 날짜가 다르면 날짜영역 보이기
		if (LastArea == null || LastArea.Time.Substring(0, 10) != Area.Time.Substring(0, 10))
		{
			AreaScript dateArea = Instantiate(DateArea).GetComponent<AreaScript>();
			dateArea.transform.SetParent(ContentRect.transform, false);
			dateArea.transform.SetSiblingIndex(dateArea.transform.GetSiblingIndex() - 1);

			string week = "";
			switch (t.DayOfWeek)
			{
				case DayOfWeek.Sunday: week = "일"; break;
				case DayOfWeek.Monday: week = "월"; break;
				case DayOfWeek.Tuesday: week = "화"; break;
				case DayOfWeek.Wednesday: week = "수"; break;
				case DayOfWeek.Thursday: week = "목"; break;
				case DayOfWeek.Friday: week = "금"; break;
				case DayOfWeek.Saturday: week = "토"; break;
			}

			dateArea.TextRect.GetComponent<Text>().text = t.Year + "년 " + t.Month + "월 " + t.Day + "일 " + week + "요일";
		}

		LastArea = Area;
		if (!isSend && !isBottom) return;
		Invoke("ScrollDelay", 0.1f);

	}

	void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

	void ScrollDelay() => scrollBar.value = 0;

}
