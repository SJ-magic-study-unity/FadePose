/************************************************************
note
	animationは、Bake into poseして置かないと、animationがobjectのtransformに影響を与えてしまう.
	つまり、animationする度、position, rotationが動くので、Loopしていくとoriginalから、ズレが生じてくる。
************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/************************************************************
************************************************************/
public class FadePose : MonoBehaviour {
	/****************************************
	****************************************/
	/******************************
	******************************/
	KeyCode Key_FadeUP		= KeyCode.UpArrow;
	KeyCode Key_FadeDown	= KeyCode.DownArrow;
	
	/******************************
	******************************/
	[System.Serializable]
	public class MODEL {
		public GameObject		GameObject;		// GameObject is a class.
		public Avatar			Avatar;			// Avatar is a class.
		
		public HumanPoseHandler	PoseHandler;	// no disp on inspector. HumanPoseHandler is a class.
		
		public HumanPose		HumanPose;		// no disp on inspector. HumanPose is a strucy.
	}
	
	public MODEL[] model_src;
	public MODEL[] model_dst;
	
	float Fade = 1.0f;
	private string label = "saijo";
	
	
	/****************************************
	****************************************/
	/******************************
	******************************/
	void Start () {
		for(int i = 0; i < model_src.Length; i++){
			model_src[i].PoseHandler = new HumanPoseHandler(model_src[i].Avatar, model_src[i].GameObject.transform);
			model_src[i].PoseHandler.GetHumanPose(ref model_src[i].HumanPose); // need this(GetHumanPose) before calling SetHumanPose : alloc memory.
		}
		for(int i = 0; i < model_dst.Length; i++){
			model_dst[i].PoseHandler = new HumanPoseHandler(model_dst[i].Avatar, model_dst[i].GameObject.transform);
			model_dst[i].PoseHandler.GetHumanPose(ref model_dst[i].HumanPose); // need this(GetHumanPose) before calling SetHumanPose : alloc memory.
		}
	}
	
	/******************************
	******************************/
 	void Update () {
		/********************
		********************/
		const float step = 0.01f;
		// if(Input.GetKeyDown(Key_FadeUP)){
		if(Input.GetKey(Key_FadeUP)){
			Fade += step;
			if(1.0f < Fade) Fade = 1.0f;
		// }else if(Input.GetKeyDown(Key_FadeDown)){
		}else if(Input.GetKey(Key_FadeDown)){
			Fade -= step;
			if(Fade < 0.0f) Fade = 0.0f;
		}
		
		/********************
		********************/
		model_src[0].PoseHandler.GetHumanPose(ref model_src[0].HumanPose);
		model_src[1].PoseHandler.GetHumanPose(ref model_src[1].HumanPose);
		
		mix_anim(ref model_src[0].HumanPose, ref model_src[1].HumanPose, ref model_dst[0].HumanPose);
		model_dst[0].PoseHandler.SetHumanPose(ref model_dst[0].HumanPose);
		
		/********************
		********************/
		
		float fps = 1.0f / Time.deltaTime;
		label =	string.Format("{0:0.000000}\n{1:0.0}\n({2:0.0}, {3:0.0}, {4:0.0}), ({5:0.0}, {6:0.0}, {7:0.0}), ({8:0.0}, {9:0.0}, {10:0.0})",
								Fade, (int)(fps + 0.5f), 
								model_src[0].HumanPose.bodyPosition.x, model_src[0].HumanPose.bodyPosition.y, model_src[0].HumanPose.bodyPosition.z,
								model_src[1].HumanPose.bodyPosition.x, model_src[1].HumanPose.bodyPosition.y, model_src[1].HumanPose.bodyPosition.z,
								model_dst[0].HumanPose.bodyPosition.x, model_dst[0].HumanPose.bodyPosition.y, model_dst[0].HumanPose.bodyPosition.z
								);
	}
	
	/******************************
	******************************/
	void mix_anim(ref HumanPose HumanPose_from, ref HumanPose HumanPose_to, ref HumanPose HumanPose_dst){
		/********************
		********************/
		for(int i = 0; i < HumanTrait.MuscleCount; i++){
			HumanPose_dst.muscles[i] = Fader(HumanPose_from.muscles[i], HumanPose_to.muscles[i], Fade);
		}
		
		/********************
		https://docs.unity3d.com/ja/current/ScriptReference/Quaternion.html
			Slerp
				a と b の間を t で球状に補間します。パラメーター t は、[0, 1] の範囲です。
				
			Lerp
				a と b の間を t で補間し、その後、その結果を正規化します。パラメーター t は、[0, 1] の範囲です。
				Slerp よりも高速ですが、回転が互いに離れている場合、見た目は劣ります。
		********************/
		HumanPose_dst.bodyRotation = Quaternion.Slerp(HumanPose_from.bodyRotation, HumanPose_to.bodyRotation, Fade);
		// HumanPose_dst.bodyRotation = Quaternion.Lerp(HumanPose_from.bodyRotation, HumanPose_to.bodyRotation, Fade);
		
		/********************
		********************/
		Vector3 pos_0 = HumanPose_from.bodyPosition;
		Vector3 pos_1 = HumanPose_to.bodyPosition;
		Vector3 pos_2 = Fader(pos_0, pos_1, Fade);
		
		HumanPose_dst.bodyPosition = pos_2;
	}
	
	/******************************
	******************************/
	float Fader(float from, float to, float ratio){
		return from * (1 - ratio) + to * ratio;
	}
	
	/******************************
	******************************/
	Vector3 Fader(Vector3 from, Vector3 to, float ratio){
		Vector3 ret = new Vector3(Fader(from.x, to.x, ratio), Fader(from.y, to.y, ratio), Fader(from.z, to.z, ratio));
		return ret;
	}
	
	/******************************
	******************************/
	void OnGUI()
	{
		GUI.color = Color.black;
		
		/********************
		本scriptは、dst側にattach.
		dstが2つ以上あると、label位置がかぶってしまうので、本出力は、debug用.
		********************/
		GUI.Label(new Rect(15, 50, 500, 60), label);
	}
}
