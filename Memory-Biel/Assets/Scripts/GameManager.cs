using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq.Expressions;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Button startButton;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI bestTime;
    public TextMeshProUGUI titolText;
    public TextMeshProUGUI intentsText;
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public GameObject[] cards;
    public GameObject[] cardSons;
    private double id_double = 0;
    private GameObject[] cardsSelected = new GameObject[2];
    private int cartesAdivinades;
    private bool clickTrigger = false;
    private float clickCooldown = 0;
    private bool startGameTrigger = false;
    private double timeNum;
    private int intentsNum;

    void Start()
    {
        cardsSelected[0] = null;
        cardsSelected[1] = null;
        cartesAdivinades = 0;
        cards = GameObject.FindGameObjectsWithTag("CardTag");
        cardSons = GameObject.FindGameObjectsWithTag("CardTagSon");

        titolText.text = "Meme Memory";
        intentsNum = 0;
        intentsText.text = "Intents: " + intentsNum;
        bestTime.text = "Millor temps: " + PlayerPrefs.GetInt("BestScore", 0);
        timeText.text = "Time: " + 0;

        if (startButton != null)
        {
            startButton.onClick.AddListener(AccionBoton);
        }
    }

    void Update()
    {
        if (startGameTrigger)
        {
            timeNum += Time.deltaTime;
            timeText.text = ("Time: " + (int)timeNum);
        }

        if (clickTrigger)
        {
            clickCooldown += Time.deltaTime;
            if (clickCooldown >= 1)
            {
                clickCooldown = 0;
                clickTrigger = false;
            }
        }

        if (cardsSelected[0] != null && cardsSelected[1] != null)
        {
            AnimatorStateInfo stateInfo0 = cardsSelected[0].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo stateInfo1 = cardsSelected[1].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

            if (stateInfo0.IsName("FigureUp") && stateInfo1.IsName("FigureUp"))
            {
                checkIds();
            }
        }

        if (cartesAdivinades == 8)
        {
            audioSource.PlayOneShot(audioClips[4]); // Final sound
            if (timeNum < PlayerPrefs.GetInt("BestScore", 0))
            {
                titolText.text = "¡¡New best score!!";
                PlayerPrefs.SetInt("BestScore", (int)timeNum);
                titolText.color = Color.yellow;
            }
            startGameTrigger = false;
            cartesAdivinades += 1;
            Invoke("FinishScene", 5);
        }
    }

    public void cardTouched(GameObject cardtouched)
    {
        if (cardsSelected[0] == null && cardsSelected[1] == null)
        {
            cardsSelected[0] = cardtouched.gameObject;
            if (audioSource != null && audioClips.Length > 0)
                audioSource.PlayOneShot(audioClips[0]); // Clicar sound
        }
        else
        {
            cardsSelected[1] = cardtouched.gameObject;
            if (audioSource != null && audioClips.Length > 0)
                audioSource.PlayOneShot(audioClips[0]); // Clicar sound
        }
    }

    public void checkIds()
    {
        if (cardsSelected[0] != null && cardsSelected[1] != null)
        {
            if (cardsSelected[0].GetComponent<CardScript>().getId() != cardsSelected[1].GetComponent<CardScript>().getId())
            {
                // Cuando las cartas no coinciden
                cardsSelected[0].GetComponent<CardScript>().esconder();
                cardsSelected[1].GetComponent<CardScript>().esconder();
                intentsNum += 1;
                intentsText.text = "Intents: " + intentsNum;
                if (audioSource != null && audioClips.Length > 1)
                    audioSource.PlayOneShot(audioClips[1]); // No, no, no sound
                borrarSeleccionados(12);
            }
            else
            {
                // Cuando las cartas coinciden
                cartesAdivinades++;
                if (audioSource != null && audioClips.Length > 2)
                    audioSource.PlayOneShot(audioClips[2]); // Adivinar sound
                borrarSeleccionados(12);
            }
        }
    }

    public bool hiHaPuesto()
    {
        return cardsSelected[0] == null || cardsSelected[1] == null;
    }

    public void borrarSeleccionados(int num)
    {
        if (num == 0)
        {
            cardsSelected[0] = null;
        }
        if (num == 1)
        {
            cardsSelected[1] = null;
        }
        if (num == 12)
        {
            cardsSelected[0] = null;
            cardsSelected[1] = null;
        }
    }

    void Mezclar(GameObject[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(i, n);
            GameObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    void AccionBoton()
    {
        if (audioSource != null && audioClips.Length > 0)
            audioSource.PlayOneShot(audioClips[0]); // Clicar sound
        cardSons[0].GetComponent<CardScript>().setStartVar(true);
        startButton.gameObject.SetActive(false);
        startGameTrigger = true;
        titolText.text = "";
        intentsNum = 0;
        intentsText.text = "Intents: " + intentsNum;

        foreach (GameObject card in cardSons)
        {
            card.GetComponent<CardScript>().setId(id_double);
            id_double = id_double + 0.5;
            Material materialCargado = Resources.Load<Material>("Materials/FigureMaterial" + card.GetComponent<CardScript>().getId());
            card.GetComponent<CardScript>().getFiguraRenderer().material = materialCargado;
        }

        Mezclar(cards);

        int i = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Vector3 posicion = new Vector3((float)((x * 2) - 2), (float)-1.3, (y * 2) - 2);
                cards[i].transform.position = posicion;
                i++;
            }
        }
    }

    void FinishScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public bool canClickTrigger()
    {
        return !clickTrigger;
    }

    public void setClickTrigger(bool clickTriger)
    {
        clickTrigger = clickTriger;
    }
}
