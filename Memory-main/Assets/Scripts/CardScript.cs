using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public GameObject figura;
    private GameObject gm;
    private int id;
    public Material image;
    public static bool startVar;

    // Start is called before the first frame update
    void Start()
    {
        startVar=false;
        gm = GameObject.FindGameObjectWithTag("GameController"); // Troba el GameManager
        Renderer renderer = figura.GetComponent<Renderer>();
        renderer.material = image;
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Listener de quan fas click a la carta.
    void OnMouseDown()
    {
        //Comprovem si ha iniciat el joc.
        if(!startVar){
            return;
        }
        if(gm.GetComponent<GameManager>().canClickTrigger()){
            gm.GetComponent<GameManager>().setClickTrigger(true);
            AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            if(stateInfo.IsName("FigureDown")){
                if(gm.gameObject.GetComponent<GameManager>().hiHaPuesto()){
                    if(stateInfo.IsName("FigureDown")){
                        Animator an = GetComponent<Animator>();
                        an.SetTrigger("FigureShowTrigger");
                    }
                    gm.GetComponent<GameManager>().cardTouched(gameObject);
                }
            }
        }
    }

    //Fer la animacio de amagat
    public void esconder(){
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("FigureUp"))
        {
            Animator an = GetComponent<Animator>();
            an.SetTrigger("FigureHideTrigger");
        }
    }

    //Fer la animacio de ensenyar la carta
    public void showFigure(){
        AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("FigureDown"))
        {
            Animator an = GetComponent<Animator>();
            an.SetTrigger("FigureShowTrigger");
        }
    }

    /*Setters i getters*/
    public Renderer getFiguraRenderer(){
        Renderer renderer = figura.GetComponent<Renderer>();
        return renderer;
    }

    public void setStartVar(bool var){
        startVar=var;
    }

    public void setId(double idp){
        id=(int)idp;
    }

    public int getId(){
        return id;
    }
    
}
