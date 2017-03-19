using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
	private float duration;
	private string message;

	public Message(string msg, float dur)
	{
		message = msg;
		duration = dur;
	}

	public string _Message {
		get {
			return message;
		}
		set {
			message = value;
		}
	}

	public float Duration {
		get {
			return duration;
		}
		set {
			duration = value;
		}
	}
}
