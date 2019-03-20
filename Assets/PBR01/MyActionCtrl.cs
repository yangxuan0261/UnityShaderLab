using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyActionCtrl : MonoBehaviour {

	private string[] _actions = {
		"attack0",
		"attack1",
		"block0",
		"block1",
		"blow0",
		"blow1",
		"blow2",
		"blow3",
		"blow4",
		"blow5",
		"blow6",
		"counter_att",
		"die",
		"fall0",
		"fall1",
		"fall2",
		"idle0",
		"idle1",
		"idle2",
		"injured0",
		"injured1",
		"move",
		"passive0",
		"passive1",
		"passive2",
		"quickstanding0",
		"quickstanding1",
		"quickstanding2",
		"skill0",
		"skill1",
		"skill2",
		"victory",
		"walk",
	};

	public enum EAct {
		attack0 = 0,
		attack1,
		block0,
		block1,
		blow0,
		blow1,
		blow2,
		blow3,
		blow4,
		blow5,
		blow6,
		counter_att,
		die,
		fall0,
		fall1,
		fall2,
		idle0,
		idle1,
		idle2,
		injured0,
		injured1,
		move,
		passive0,
		passive1,
		passive2,
		quickstanding0,
		quickstanding1,
		quickstanding2,
		skill0,
		skill1,
		skill2,
		victory,
		walk,
	}

	private EAct oldAct = EAct.idle0;
	public EAct newAct = EAct.idle0;
	public Animator _animtor1;	
	public Animator _animtor2;	

	// Use this for initialization
	void Start () {
		SetAct(newAct);
	}
	
	// Update is called once per frame
	void Update () {
		if (oldAct != newAct) {
			oldAct = newAct;
			SetAct(newAct);
		}
	}

	void SetAct(EAct act) {
		string actStr = _actions[(int)act];
		if (_animtor1 != null) {
			_animtor1.SetTrigger(actStr);
		}

		if (_animtor2 != null) {
			_animtor2.SetTrigger(actStr);
		}
	}
}
