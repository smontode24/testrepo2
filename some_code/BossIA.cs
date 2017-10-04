
using UnityEngine;
using System.Collections;

public class BossIA : MonoBehaviour {
	public bool onStay;
	public bool onMove;
	public bool saltoMedio;
	public bool canAttackMid;
	public bool jump;
	public bool pointArrived;
	public bool stepped;
	public bool canAttack;
	public static bool ball1AvaibleToHit;
	public static bool ball2AvaibleToHit;
	public static bool ball3AvaibleToHit;
	public static bool ball4AvaibleToHit;
	public bool ground;
	private bool particleGround;
	private bool changedMaterial2;
	private bool changedMaterial3;
	public OnBossEnter onBossEnter;
	public Transform[] points;
	public Transform[] points2;
	public Transform corazonSpawner;
	public GameObject boss;
	public GameObject cubeonbossenter;
	public GameObject player;
	public GameObject cylinder;
	public GameObject cylinderTrigger;
	public GameObject ball1;
	public GameObject ball2;
	public GameObject ball3;
	public GameObject ball4;
	public GameObject ballCabeza;
	public GameObject polvo;
	public GameObject polvo2;
	public GameObject particles;
	public GameObject particles2;
	public GameObject particleSource;
	public int numPoint;
	public int lastPoint;
	public int numPoint2;
	public int lastPoint2;
	public int x;
	public int y;
	public float cooldownMove = 10f;
	public float cooldownTime;
	public float cooldownTimeFire;
	public float cooldownFire;
	public float cooldownMid = 35f;
	public float cooldownMidMove;
	public float smoothMove = 0.8f;
	public float smoothRotate = 3f;
	public float speed = 250f;
	public float distGround = 2.01f;
	public float smoothMove2 = 0.5f;
	public float distance;
	private Quaternion targetRotation;
	private Vector3 velocity = Vector3.zero;
	public Rigidbody corazonPrefab;
	public Rigidbody corazonPrefab2;
	public Rigidbody corazon1;
	public Rigidbody corazon2;
	public Rigidbody corazon3;
	public Transform corazon1_transform;
	public Transform corazon2_transform;
	public Transform corazon3_transform;
	public float corazonSpeed = 2500f;
	private Vector3 toLeft;
	private Vector3 toRight;
	public float jumpPower;
	public float jumpPower2;
	public int i;
	public AtaqueMedioBossTrigger attackMiddle;
	public Boss1Cabeza boss1cabezaScript;
	public ControlsOnBoss controls;
	public static int ballsHitted;
	public Material rojo;
	public Material blanco;
	public Material normal;
	public Material atacando;
	public Material fase2;
	public Material fase3;
	public GameObject dieGUI;
	public Rigidbody corazonMesh;
	public Rigidbody corazonMeshInstantiate1;
	public Rigidbody corazonMeshInstantiate2;
	public Rigidbody corazonMeshInstantiate3;
	public OnTerrenoBoss onterreno;
	public GameObject cubeTerreno;
	public AudioSource dirt;
	public AudioSource nyan;
	public AudioSource nyaaaaaan;
	public AudioSource nyanEnfadado;
	public AudioSource nyaaaaaanEnfadado;
	//public GameObject corazonMesh2;
	//public GameObject corazonMesh3;
	// Use this for initialization
	void Start () {
		attackMiddle = cylinderTrigger.GetComponent<AtaqueMedioBossTrigger>();
		onBossEnter = cubeonbossenter.GetComponent<OnBossEnter> ();
		x = 0;
		onStay = true;
		cooldownTime = 0f;
		lastPoint = 2;
		y = 1;
		i = 0;
		cooldownMidMove = cooldownMid + 10f;
		jump = false;
		ballsHitted = 0;
		ball1AvaibleToHit = false;
		ball2AvaibleToHit = false;
		ball3AvaibleToHit = false;
		ball4AvaibleToHit = false;
		particleGround = false;
		changedMaterial2 = false;
		changedMaterial3 = false;
		pointArrived = true;
		stepped = true;
		canAttack = true;
		boss1cabezaScript = ballCabeza.GetComponent<Boss1Cabeza> ();
		controls = player.GetComponent<ControlsOnBoss> ();
		onterreno = cubeTerreno.GetComponent<OnTerrenoBoss>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!OnBossEnter.onBossEnter){
			//if(OnTerrenoBoss.onTerreno == true){
			//Primeras fases hasta que solo queda la cabeza
			if(ballsHitted != 4 && ballsHitted != 5){
				
				if(onStay == true && canAttackMid == false){//Boss siempre mira al jugador cuando esta quieto
					targetRotation = Quaternion.LookRotation(player.transform.position - boss.transform.position);
					targetRotation = Quaternion.Euler(0,targetRotation.eulerAngles.y,0);
					boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation,targetRotation,smoothRotate * Time.deltaTime);
					
				}
				//Dispara
				if(onStay == true && y == 0 &&  canAttackMid == false){
					StartCoroutine(Disparar());
					
					y = 1;
				}
				//Ataque al medio
				if(x == 0 && AtaqueMedioBossTrigger.attackMid == true){
					if(Time.time > cooldownMidMove){
						canAttackMid = true;
						x = 1;
						numPoint = 5;
						cooldownMidMove = Time.time + cooldownMid;
						StartCoroutine(MoveMiddle());
						x = 2;
						
						
					}
				}
				//Cuando pasa cierto tiempo estando quieto, se elije otra posicion con movePoints()
				if(onStay == true && x == 0 &&  canAttackMid == false){// Escoge un valor aleatorio de posicion
					if(Time.time > cooldownTime){
						cooldownTime = Time.time + cooldownMove;
						movePoints();
						x = 1;
					}
				}
				if(x == 1 &&  canAttackMid == false){// Moverse a dicha posicion
					StartCoroutine(Move());
					x = 2;
					
				}
				//Moverse a la direccion elegida de forma aleatoria
				if(onMove == true && canAttackMid == false){
					boss.transform.position = Vector3.SmoothDamp (boss.transform.position, points[numPoint].position, ref velocity, smoothMove);
					boss.transform.LookAt(points[numPoint].transform);
				}
				//Ataque al medio ejecutar
				if(onMove == true && canAttackMid == true){
					boss.transform.position = Vector3.SmoothDamp (boss.transform.position, points[numPoint].position, ref velocity, smoothMove);
					boss.transform.rotation = Quaternion.Euler (0f,boss.transform.rotation.eulerAngles.y,0f);
					if(jump == false){
						boss.GetComponent<Rigidbody> ().AddForce (Vector3.up * jumpPower);
						StartCoroutine(JumpForce2());
						jump = true;
					}

				}else{
					jump = false;
				}
				//Augmentar velocidad al atacar al jugador
				if(numPoint == 4){
					smoothMove = 0.4f;
				}
				else{
					smoothMove = 0.8f;
				}
				//Particulas
				if(Time.timeScale > 0){
				if(onMove == true && canAttackMid == false){
					particles = Instantiate(polvo,particleSource.transform.position,new Quaternion(0,0,0,0)) as GameObject;
					if(dirt.isPlaying == false){
						dirt.Play ();
						}
					if(nyaaaaaan.isPlaying == false){
						nyaaaaaan.Play ();
							}
				}
				}
				//Detectar cuando cae al suelo para spawnear las particulas despues del salto
				Ray ray = new Ray (transform.position, Vector3.down);
				int myLay = 1 << 9;
				myLay = ~myLay;
				if (Physics.Raycast (ray, distGround, myLay)) {
					ground = true;
				} else {
					ground = false;
				}
				if (ground == true && particleGround == true){
					particles2 = Instantiate (polvo2, particleSource.transform.position + new Vector3 (0,-10,0), new Quaternion (0, 0, 0, 0)) as GameObject;
					particleGround = false;
				}
				if(ballsHitted == 2 && changedMaterial2 == false){
					ballCabeza.GetComponent<MeshRenderer>().material = fase2;
					changedMaterial2 = true;
				}
				if(ballsHitted == 4 && changedMaterial3 == false){
					ballCabeza.GetComponent<MeshRenderer>().material = fase3;
					changedMaterial3 = true;
				}
				//Mesh corazon misma posicion con el modelo
				if(corazonMeshInstantiate1 != null && corazon1 != null){
					corazonMeshInstantiate1.transform.position = corazon1.transform.position;
					corazonMeshInstantiate1.transform.localScale = corazon1.transform.localScale;
				}
				if(corazonMeshInstantiate2 != null && corazon2 != null){
					corazonMeshInstantiate2.transform.position = corazon2.transform.position;
					corazonMeshInstantiate2.transform.localScale = corazon2.transform.localScale;
				}
				if(corazonMeshInstantiate3 != null && corazon3 != null){
					corazonMeshInstantiate3.transform.position = corazon3.transform.position;
					corazonMeshInstantiate3.transform.localScale = corazon3.transform.localScale;
				}

			}
			//Fase final
			if(ballsHitted == 4){
				if(ballCabeza != null){
				boss.GetComponent <BoxCollider>().isTrigger = true;
				boss.GetComponent<Rigidbody>().useGravity = false;
				if(pointArrived == true && stepped == true){
					pointArrived = false;
					movePoints2 ();
					stepped = false;
				}
				if(ballCabeza != null && stepped == false){
					canAttack = true;
				}
				if (pointArrived == false && stepped == false){
					stepped = true;
					StartCoroutine(NextStep());

				}
				if(pointArrived == false){
					boss.transform.position = Vector3.SmoothDamp (boss.transform.position, points2[numPoint2].position, ref velocity, smoothMove);
					boss.transform.LookAt(points2[numPoint2].transform);
				}
				if(canAttack == true){
					canAttack = false;
					StartCoroutine(AtacarFaseFinal());
				}
				if(Boss1Cabeza.boss1Defeat == true && corazon1 != null){
						Destroy(corazon1);
					}
				}
				if(corazonMeshInstantiate1 != null && corazon1 != null){
					corazonMeshInstantiate1.transform.position = corazon1.transform.position;
					corazonMeshInstantiate1.transform.localScale = corazon1.transform.localScale;
				}
			}
		//}else if (OnTerrenoBoss.onTerreno == false){
		//		boss.transform.position = points[numPoint].position;
			//}
		}
	}

	IEnumerator AtacarFaseFinal(){
		yield return new WaitForSeconds (1.9f);
		Fire2 ();
		}
	//Tiempo de espera antes de ir hacia otro punto en la ultima fase
	IEnumerator NextStep(){
		yield return new WaitForSeconds (2f);
		pointArrived = true;
	}
	// Transicion de movimiento
	IEnumerator Move(){
		onStay = false;
		onMove = true;
		yield return new WaitForSeconds (2f);
		onStay = true;
		onMove = false;
		x = 0;
		y = 0;
	}
	//Transicion de salto al medio
	IEnumerator MoveMiddle(){
		onStay = false;
		onMove = true;
		yield return new WaitForSeconds (1f);
		nyaaaaaan.Play ();
		particleGround = true;
		yield return new WaitForSeconds (3f);
		onStay = true;
		onMove = false;
		switch (ballsHitted) {
			//Poner aqui si hittea
		case 0:
			ball1.GetComponent<MeshRenderer>().material = rojo;
			ball1AvaibleToHit = true;
			break;
		case 1:
			ball2.GetComponent<MeshRenderer>().material = rojo;
			ball2AvaibleToHit = true;
			break;
		case 2:
			ball3.GetComponent<MeshRenderer>().material = rojo;
			ball3AvaibleToHit = true;
			break;
		case 3:
			ball4.GetComponent<MeshRenderer>().material = rojo;
			ball4AvaibleToHit = true;
			break;
		}
		yield return new WaitForSeconds (1f);
		boss.GetComponent<Animation>().Play("aparece");
		yield return new WaitForSeconds (1f);
		boss.GetComponent<Animation>().Play("aparece");
		yield return new WaitForSeconds (1f);
		boss.GetComponent<Animation>().Play("aparece");
		yield return new WaitForSeconds (1f);
		boss.GetComponent<Animation>().Play("aparece");
		switch (ballsHitted) {
			//Poner aqui si hittea
		case 0:
			if(ball1 != null){
				ball1.GetComponent<MeshRenderer>().material = blanco;
				ball1AvaibleToHit = false;
			}
			break;
		case 1:
			if(ball2 != null){
				ball2.GetComponent<MeshRenderer>().material = blanco;
				ball2AvaibleToHit = false;
			}
			break;
		case 2:
			if(ball3 != null){
				ball3.GetComponent<MeshRenderer>().material = blanco;
				ball3AvaibleToHit = false;
			}
			break;
		case 3:
			if(ball4 != null){
				ball4.GetComponent<MeshRenderer>().material = blanco;
				ball4AvaibleToHit = false;
			}
			break;
		}
		x = 0;
		y = 0;
		canAttackMid = false;
	}
	//Añade una fuerza hacia abajo para caer antes del salto
	IEnumerator JumpForce2(){
		yield return new WaitForSeconds (2f);
		boss.GetComponent<Rigidbody> ().AddForce (Vector3.down * jumpPower2);
	}
	//Elige un punto aleatorio
	void movePoints(){
		if(canAttackMid == false){
			numPoint = Random.Range (0, 5);
			while (numPoint == lastPoint) {
				numPoint = Random.Range(0,5);
			}
			lastPoint = numPoint;
		}
	}
	//Puntos en la ultima fase
	void movePoints2(){
		
		numPoint2 = Random.Range (0, 5);
		while (numPoint2 == lastPoint2) {
			numPoint2 = Random.Range(0,5);
		}
		lastPoint2 = numPoint2;
		
	}
	//Dispara
	IEnumerator Disparar(){
		yield return new WaitForSeconds (1.5f);
		Fire ();
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = atacando;
		}
		yield return new WaitForSeconds (0.5f);
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = normal;
		}
		yield return new WaitForSeconds (1.5f);
		Fire ();
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = atacando;
		}
		yield return new WaitForSeconds (0.5f);
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = normal;
		}
		yield return new WaitForSeconds (1.5f);
		Fire ();
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = atacando;
		}
		yield return new WaitForSeconds (0.5f);
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = normal;
		}
		yield return new WaitForSeconds (1.5f);
		Fire ();
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = atacando;
		}
		yield return new WaitForSeconds (0.5f);
		if (ballsHitted == 0 || ballsHitted == 1) {
			ballCabeza.GetComponent<MeshRenderer> ().material = normal;
		}
	}
	//Genera los tres proyectiles
	void Fire (){
		if(canAttackMid == false){
		nyan.Play ();
		corazon1 = (Rigidbody)Instantiate (corazonPrefab, corazon1_transform.position, corazon1_transform.rotation);
		corazon2 = (Rigidbody)Instantiate (corazonPrefab, corazon2_transform.position, corazon1_transform.rotation);
		corazon3 = (Rigidbody)Instantiate (corazonPrefab, corazon3_transform.position, corazon1_transform.rotation);

		corazonMeshInstantiate1 = (Rigidbody)Instantiate (corazonMesh, corazon1.transform.position, corazon1.transform.rotation);
		corazonMeshInstantiate2 = (Rigidbody)Instantiate (corazonMesh, corazon1.transform.position, corazon1.transform.rotation);
		corazonMeshInstantiate3 = (Rigidbody)Instantiate (corazonMesh, corazon1.transform.position, corazon1.transform.rotation);


		toRight = Quaternion.Euler (0, - 10, 0) * transform.forward;
		toLeft = Quaternion.Euler (0, 10, 0) * transform.forward;
		
		corazon1.GetComponent<Rigidbody> ().AddForce (transform.forward * corazonSpeed / Time.deltaTime);
		corazon2.GetComponent<Rigidbody> ().AddForce (toRight * corazonSpeed / Time.deltaTime);
		corazon3.GetComponent<Rigidbody> ().AddForce (toLeft * corazonSpeed / Time.deltaTime);
		}
	}
	//Genera los tres proyectiles en la segunda fase
	void Fire2 (){
		if(Boss1Cabeza.boss1Defeat == false){
		nyanEnfadado.Play();
		Quaternion corazonRotation = Quaternion.LookRotation (player.transform.position - corazonSpawner.position);
		corazon1 = (Rigidbody)Instantiate (corazonPrefab2, corazonSpawner.position, corazonRotation);
		Vector3 direccion = player.transform.position - corazonSpawner.position;
		direccion.Normalize();
		corazonMeshInstantiate1 = (Rigidbody)Instantiate (corazonMesh, corazon1.transform.position, corazon1.transform.rotation);

		corazon1.GetComponent<Rigidbody> ().AddForce (direccion * corazonSpeed/Time.deltaTime); 
		}

	}
	void OnCollisionEnter(Collision other){
		if (other.transform.tag == "Player") {
			ControlsOnBoss.Die(dieGUI);
				}

	}

}
