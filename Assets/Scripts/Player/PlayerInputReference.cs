using UnityEngine;
using System.Collections;

public static class PlayerInputReference{

	public enum PlayerCommand {
		jump
	}
	public static string getPlayerCommand(int playerNumber, PlayerCommand playerCommand) {

		switch (playerCommand) {
		case PlayerCommand.jump: return "Jump" + playerNumber;
			break;
		}
		return "";
	}


}

