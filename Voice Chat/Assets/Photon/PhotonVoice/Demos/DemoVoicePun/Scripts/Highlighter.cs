// ----------------------------------------------------------------------------
// <copyright file="Highlighter.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Class that highlights the Photon Voice features by toggling isometric view 
// icons for the two components Recorder and Speaker.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value

namespace ExitGames.Demos.DemoPunVoice
{

    using UnityEngine;
    using UnityEngine.UI;
	using Photon.Voice.Unity;
	using Photon.Voice.PUN;

    [RequireComponent(typeof(Canvas))] // 캐릭터 머리 위 캔버스에 붙는 스크립트
    public class Highlighter : MonoBehaviour
    {
        private Canvas canvas;

        private PhotonVoiceView photonVoiceView;

        [SerializeField]
        private Image recorderSprite; // 말할때 내 위에 뜨는 이미지

        [SerializeField]
        private Image speakerSprite; // 나에게 들릴때 상대방 위에 뜨는 이미지

        [SerializeField]
        private Text bufferLagText; // Debug 모드에서 속도지연 띄우는거

        private bool showSpeakerLag;

        private void OnEnable()
        {
            ChangePOV.CameraChanged += this.ChangePOV_CameraChanged;
            VoiceDemoUI.DebugToggled += this.VoiceDemoUI_DebugToggled;
        }

        private void OnDisable()
        {
            ChangePOV.CameraChanged -= this.ChangePOV_CameraChanged;
            VoiceDemoUI.DebugToggled -= this.VoiceDemoUI_DebugToggled;
        }

        private void VoiceDemoUI_DebugToggled(bool debugMode) // 디버그모드 토글 시
        {
            this.showSpeakerLag = debugMode;
        }

        private void ChangePOV_CameraChanged(Camera camera) // 버튼으로 카메라 변경시
        {
            this.canvas.worldCamera = camera;
        }

        private void Awake()
        {
            this.canvas = this.GetComponent<Canvas>(); 
            if (this.canvas != null && this.canvas.worldCamera == null) { this.canvas.worldCamera = Camera.main; }
            this.photonVoiceView = this.GetComponentInParent<PhotonVoiceView>();
        }


        // Update is called once per frame
        private void Update()
        {
            this.recorderSprite.enabled = this.photonVoiceView.IsRecording; // 내가 말하는 중일때 아이콘 띄우기
            this.speakerSprite.enabled = this.photonVoiceView.IsSpeaking; // 상대방으로서 말하는 중일때 아이콘 띄우기
            this.bufferLagText.enabled = this.showSpeakerLag && this.photonVoiceView.IsSpeaking;
            if (this.bufferLagText.enabled)
            {
                this.bufferLagText.text = string.Format("{0}", this.photonVoiceView.SpeakerInUse.Lag);
            }
        }

        //가장 마지막에 호출되는것으로 주로 오브젝트를 따가가게 설정한 카메라에 사용된다.
        private void LateUpdate()
        {
            if (this.canvas == null || this.canvas.worldCamera == null) { return; } // should not happen, throw error

            // 타인의 시선에서 봤을때 해당 카메라의 각도와 무관하게 내 위의 아이콘이 정면으로 보이게끔 transform시킴
            this.transform.rotation = Quaternion.Euler(0f, this.canvas.worldCamera.transform.eulerAngles.y, 0f); //canvas.worldCamera.transform.rotation;
            
        }
    }
}