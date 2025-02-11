using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
   public void PlayGame()
   {
        SceneManager.LoadSceneAsync(3);
   }

   public void QuitGame()
    {
        Application.Quit();
    }
   
   public void CreditsButton()
   {
        SceneManager.LoadSceneAsync("SCR_Credits");
   }

   public void BackButton()
   {
        SceneManager.LoadSceneAsync("SCR_Start");
   }
}
