using UnityEngine;
using System.Collections;

public class DelegateMenu : MonoBehaviour 
{
        //we have to declare a delegate so that we can use it in the OnGUI function
        private delegate void MenuDelegate();

        //in order to use a delegate we need to create a variable of MenuDelegate type so that it can be used throughout the code.
        private MenuDelegate menuFunction;
 
        //these variables are only here because it is cheaper to access a value from memory instead of through a static class
        private float screenHeight;
        private float screenWidth;
        private float buttonHeight;
        private float buttonWidth;
 
        // Use this for initialization
        void Start ()
        {
                screenHeight = Screen.height;
                screenWidth = Screen.width;
  
                buttonHeight = screenHeight * 0.3f;
                buttonWidth = screenWidth * 0.4f;
  
                //here we set the menuFunction to point to the anyKey function, which is further down in the code
                menuFunction = anyKey;
        }
 
        void OnGUI()
        {
        //in order to use a delegate we just call it like a function. Simple!
                menuFunction();
        }
 
        //in order to change the GUI we just change the function that menuFunction points to. It will basically take care of itself from that point.
        void anyKey()
        {
                //check if the user pressed anything, if it did, change the menuFunction to show the main menu
                if(Input.anyKey)
                {
                        menuFunction = mainMenu;
                }
  
                //this is just text in the center of the screen telling the user to press any key
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(screenWidth * 0.45f, screenHeight * 0.45f, screenWidth * 0.1f, screenHeight * 0.1f), "Press any key to continue");
        }
 
        //mainMenu only has two buttons in the version, one to play the game, and one to quit the game
        void mainMenu()
        {
                if(GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.1f, buttonWidth, buttonHeight), "Start Game"))
                {
                        Application.LoadLevel(1);
                }
  
                if(GUI.Button(new Rect((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.5f, buttonWidth, buttonHeight), "Quit Game"))
                {
                        Application.Quit();
                }
        }
}