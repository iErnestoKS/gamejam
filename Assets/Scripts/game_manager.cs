using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class game_manager : MonoBehaviour {

    int clickCount = 0;
    int xAngle = 0;
    int currentLevel = 0;
    bool timerOn = false;
    bool allTrue = false;

    public VisualElement root;
    public GameState main_state;
    public GameState playing_state;
    public Level[] levels;
    VisualElement state_lost;
    VisualElement state_win;
    VisualElement state_config;
    VisualElement state_playing;
    VisualElement state_main_menu;
    Label trys;
    Label time;
    Button game_home;
    Button game_home_win;
    Button game_exit;
    Button game_start;
    Button game_replay;
    Button game_replay_win;
    Button game_options;

    void Start() {

        main_state.id = "main_menu";
        root = GetComponent<UIDocument>().rootVisualElement;

        state_win = root.Q<VisualElement>("state_win");
        state_lost = root.Q<VisualElement>("state_lost");
        state_config = root.Q<VisualElement>("state_config");
        state_playing = root.Q<VisualElement>("state_playing");
        state_main_menu = root.Q<VisualElement>("state_main_menu");

        trys = root.Q<Label>("trys_count");
        time = root.Q<Label>("time_left");

        game_exit = root.Q<Button>("game_exit");
        game_home = root.Q<Button>("game_home");
        game_start = root.Q<Button>("game_start");
        game_replay = root.Q<Button>("game_replay");
        game_options = root.Q<Button>("game_options");
        game_home_win = root.Q<Button>("game_home_win");
        game_replay_win = root.Q<Button>("game_replay_win");

        game_exit.clicked += gameExitPressed;
        game_home.clicked += gameHomePressed;
        game_home_win.clicked += gameHomePressed;
        game_start.clicked += gameStartPressed;
        game_replay.clicked += gameReplayPressed;
        game_replay_win.clicked += gameReplayPressed;
        game_options.clicked += gameOptionsPressed;
    }
    void Update() {

        switch(main_state.id) {
            case "main_menu":
                state_win.style.display = DisplayStyle.None;
                state_lost.style.display = DisplayStyle.None;
                state_config.style.display = DisplayStyle.None;
                state_playing.style.display = DisplayStyle.None;
                state_main_menu.style.display = DisplayStyle.Flex;
                timerOn = true;
                allTrue = false;

                foreach(var level in levels) {
                    level.trys = level.default_trys;
                    level.time_left = level.default_time_left;

                    for (int i = 0; i < level.pipes_n_wires.Length; i++) {
                        level.pipes_n_wires[i] = false;
                    }
                }
            break;
            case "playing":
                state_win.style.display = DisplayStyle.None;
                state_lost.style.display = DisplayStyle.None;
                state_config.style.display = DisplayStyle.None;
                state_playing.style.display = DisplayStyle.Flex;
                state_main_menu.style.display = DisplayStyle.None;

                if (levels[currentLevel].isTry) {
                    trys.style.display = DisplayStyle.Flex;
                    time.style.display = DisplayStyle.None;
                    trys.text = $"Tentativas: {levels[currentLevel].trys}";

                    OnClickTry();
                } else {
                    trys.style.display = DisplayStyle.None;
                    time.style.display = DisplayStyle.Flex;

                    if (timerOn) {
                        if (levels[currentLevel].time_left > 0) {
                            levels[currentLevel].time_left -= Time.deltaTime;
                            updateTimer(levels[currentLevel].time_left);
                        } else {
                            levels[currentLevel].time_left = 0;
                            timerOn = false;
                            main_state.id = "lost";
                        }
                    }

                    OnClickTime();
                }

                allTrue = Array.TrueForAll(levels[currentLevel].pipes_n_wires, x => x == true);

                if(allTrue == true && levels[currentLevel].trys > 0 || allTrue == true && levels[currentLevel].time_left > 0) {
                    main_state.id = "win";
                    currentLevel++;
                }
            break;
            case "lost":
                state_win.style.display = DisplayStyle.None;
                state_lost.style.display = DisplayStyle.Flex;
                state_config.style.display = DisplayStyle.None;
                state_playing.style.display = DisplayStyle.None;
                state_main_menu.style.display = DisplayStyle.None;
            break;
            case "win":
                state_win.style.display = DisplayStyle.Flex;
                state_lost.style.display = DisplayStyle.None;
                state_config.style.display = DisplayStyle.None;
                state_playing.style.display = DisplayStyle.None;
                state_main_menu.style.display = DisplayStyle.None;
            break;
            case "config":
                state_win.style.display = DisplayStyle.None;
                state_lost.style.display = DisplayStyle.None;
                state_config.style.display = DisplayStyle.Flex;
                state_playing.style.display = DisplayStyle.None;
                state_main_menu.style.display = DisplayStyle.None;
            break;
        }
 

    }

    void gameStartPressed() {
        main_state.id = "playing";
    }
    void gameExitPressed() {
        Application.Quit();
    }
    void gameOptionsPressed() {
        main_state.id = "config";
    }
    void gameHomePressed() {
        main_state.id = "main_menu";
    }
    void gameReplayPressed() {
        main_state.id = "playing";
    }
    void OnClickTry() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 50f)) {
                if (hit.transform != null) {
                    if (hit.transform.gameObject.layer == 6) {

                        levels[currentLevel].trys--;

                        if (levels[currentLevel].trys == 0) {
                            main_state.id = "lost";
                            return;
                        }

                        if (hit.transform.eulerAngles.x == 90) {
                            levels[currentLevel].pipes_n_wires[hit.transform.GetComponent<info>().id] = true;
                        } else {
                            levels[currentLevel].pipes_n_wires[hit.transform.GetComponent<info>().id] = false;
                        }

                        clickCount++;

                        if (clickCount == 4) {
                            clickCount = 0;
                        }

                        switch (clickCount) {
                            case 0:
                                xAngle = 0;
                            break;
                            case 1:
                                xAngle = 90;
                            break;
                            case 2:
                                xAngle = 180;
                            break;
                            case 3:
                                xAngle = 270;
                            break;
                        }

                        Vector3 rotationToAdd = new Vector3(xAngle, 0, 0);

                        hit.transform.Rotate(rotationToAdd);
                        // hit.transform.rotation = Quaternion.Slerp(hit.transform.rotation, new Quaternion(xAngle,0,0,0), Time.deltaTime);
                        // hit.transform.rotation = Quaternion.AngleAxis(xAngle, transform.right);
                        // hit.transform.rotation = Quaternion.Euler(xAngle, 0, 0);

                        print($" valor: {xAngle}");
                        print($" rotation: {hit.transform.localRotation}");
                        print($" euler: {hit.transform.eulerAngles}");
                        print($" x: {hit.transform.eulerAngles.x}");
                        print($" y: {hit.transform.eulerAngles.y}");
                        print($" z: {hit.transform.eulerAngles.z}");

                        // hit.transform.Rotate(Mathf.Abs(clickCount * 90), 0f, 0f, Space.World);
                    }
                }
            }
        }
    }

    void OnClickTime() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 50f)) {
                if (hit.transform != null) {
                    if (hit.transform.gameObject.layer == 6) {
                        
                        if (hit.transform.eulerAngles.x == 90) {
                            levels[currentLevel].pipes_n_wires[hit.transform.GetComponent<info>().id] = true;
                        } else {
                            levels[currentLevel].pipes_n_wires[hit.transform.GetComponent<info>().id] = false;
                        }

                        clickCount++;

                        if (clickCount == 4) {
                            clickCount = 0;
                        }
                        print(hit.transform.rotation);
                        print(hit.transform.rotation.eulerAngles);
                        // hit.transform.rotation = Quaternion.Euler(Mathf.Abs(clickCount * 90),hit.transform.rotation.eulerAngles.y, hit.transform.rotation.eulerAngles.z);
                    }
                }
            }
        }
    }

    void updateTimer(float currentTime) {
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        time.text = $"Tempo Restante: {string.Format("{0:00}:{1:00}", minutes, seconds)}";
    }

}