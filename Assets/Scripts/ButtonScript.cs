using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    Animator animator; //the animator to change the animation stage

    public int targetScene; //what scene to go to when clicked

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {

    }

    public void OnPointerClick(PointerEventData eventData) {
        animator.SetTrigger("clicked");

        StartCoroutine(LoadSceneAsync(targetScene));
    }

    public void OnPointerEnter(PointerEventData eventData) {
        animator.SetBool("hovered", true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        animator.SetBool("hovered", false);
    }

    IEnumerator LoadSceneAsync(int sceneNum) { //load scene async to not bog down the main thread
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNum);

        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
