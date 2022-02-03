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
		//if (text.Trim() == "") return; // ���� ���ڿ��� ����

		bool isBottom = scrollBar.value <= 0.00001f; // ���� ��ũ�ѹٰ� �� �ؿ� ��ġ���ִ���
		//print(text);

		//������ ����� ���, �޴� ����� ��������� ����� �ؽ�Ʈ ����
		AreaScript Area = Instantiate(isSend ? YellowArea : WhiteArea).GetComponent<AreaScript>();
		Area.transform.SetParent(ContentRect.transform, false);
		Area.BoxRect.sizeDelta = new Vector2(180, Area.BoxRect.sizeDelta.y);
		Area.TextRect.GetComponent<Text>().text = text;
		Fit(Area.BoxRect);
	

		// ���α��� ���� (max �ʺ� ���� ª�� ��� ���)
		float X = Area.TextRect.sizeDelta.x + 21;
		float Y = Area.TextRect.sizeDelta.y;

		if (Y > 15) // �� �� �ʰ�
		{
			for (int i = 0; i < 200; i++)
			{
				Area.BoxRect.sizeDelta = new Vector2(X - i * 2, Area.BoxRect.sizeDelta.y);
				Fit(Area.BoxRect);

				if (Y != Area.TextRect.sizeDelta.y) { Area.BoxRect.sizeDelta = new Vector2(X - (i * 2) + 2, Y); break; }
			}
		}
		else Area.BoxRect.sizeDelta = new Vector2(X, Y); // �� ��

		//�޼��� �� �ð� ����
		DateTime t = DateTime.Now;
		Area.Time= t.ToString("yyyy-MM-dd-HH-mm");
		Area.User = user;

		int hour = t.Hour;

		if (hour == 0) hour = 12;
		else if (hour > 12) hour -= 12;
		Area.TimeText.text = (t.Hour < 12 ? "���� " : "���� ") + hour + ":" + t.ToString("mm");

		// ���� �ð��� ������ ���� ���ֱ�
		bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.User == Area.User;
		if (isSame) LastArea.TimeText.text = "";
		Area.Tail.SetActive(!isSame);

		// - ���� �޼����� ��� �̸�, ������ ������ ���ֱ�
		if (!isSend)
		{
			Area.UserImage.gameObject.SetActive(!isSame);
			Area.UserText.gameObject.SetActive(!isSame);
			Area.UserText.text = user;
		}


		// ��¥ �ڽ�: ���� �Ͱ� ��¥�� �ٸ��� ��¥���� ���̱�
		if (LastArea == null || LastArea.Time.Substring(0, 10) != Area.Time.Substring(0, 10))
		{
			AreaScript dateArea = Instantiate(DateArea).GetComponent<AreaScript>();
			dateArea.transform.SetParent(ContentRect.transform, false);
			dateArea.transform.SetSiblingIndex(dateArea.transform.GetSiblingIndex() - 1);
			

			string week = "";
			switch (t.DayOfWeek)
			{
				case DayOfWeek.Sunday: week = "��"; break;
				case DayOfWeek.Monday: week = "��"; break;
				case DayOfWeek.Tuesday: week = "ȭ"; break;
				case DayOfWeek.Wednesday: week = "��"; break;
				case DayOfWeek.Thursday: week = "��"; break;
				case DayOfWeek.Friday: week = "��"; break;
				case DayOfWeek.Saturday: week = "��"; break;
			}

			dateArea.TextRect.GetComponent<Text>().text = t.Year + "�� " + t.Month + "�� " + t.Day + "�� "+week+"����";
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
