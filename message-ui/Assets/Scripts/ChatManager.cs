using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;



public class ChatManager : MonoBehaviour
{	
	public GameObject YellowArea, WhiteArea, DateArea;
	public RectTransform ContentRect;
	public Scrollbar scrollBar;
	public InputField inputfield;
	AreaScript LastArea;
	Test testScript;

	void Start()
	{
		inputfield.onEndEdit.AddListener(delegate { onInputSend(); });
	}


	public void chat(bool isSend, string text, string user, Texture picture)
	{
		//if (text.Trim() == "") return; // 공백 문자열은 제외

		bool isBottom = scrollBar.value <= 0.00001f; // 현재 스크롤바가 맨 밑에 위치해있는지
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
		Area.Time= t.ToString("yyyy-MM-dd-HH-mm");
		Area.User = user;

		int hour = t.Hour;

		if (hour == 0) hour = 12;
		else if (hour > 12) hour -= 12;
		Area.TimeText.text = (t.Hour < 12 ? "오전 " : "오후 ") + hour + ":" + t.ToString("mm");

		// 이전 시간과 같으면 꼬리 없애기
		bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.User == Area.User;
		if (isSame) LastArea.TimeText.text = "";
		Area.Tail.SetActive(!isSame);

		// - 받은 메세지의 경우 이름, 프로필 사진도 없애기
		if (!isSend)
		{
			Area.UserImage.gameObject.SetActive(!isSame);
			Area.UserText.gameObject.SetActive(!isSame);
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

			dateArea.TextRect.GetComponent<Text>().text = t.Year + "년 " + t.Month + "월 " + t.Day + "일 "+week+"요일";
		}

		LastArea = Area;
		if (!isSend && !isBottom) return;
		Invoke("ScrollDelay", 0.1f);
		print(isBottom);
		print(scrollBar.value);

	}

	void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

	void ScrollDelay() => scrollBar.value = 0;

	public void onInputSend()
	{
		if (inputfield.text.Trim() != "")
		{
			chat(false, inputfield.text, "seung", null);
			inputfield.text = "";
			inputfield.Select();
			inputfield.ActivateInputField();
		}		
	}
	
}
