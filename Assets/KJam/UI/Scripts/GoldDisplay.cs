﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldDisplay : MonoBehaviour
{
	private Text Text;

    void Start()
    {
		Text = GetComponent<Text>();
    }

    void Update()
    {
		Text.text = Player.Instance.GetGold() + "G";
    }
}
