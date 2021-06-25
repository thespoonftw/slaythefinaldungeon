using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatantView : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] SpriteRenderer sprite;

    private bool isFlickerOn;
    private bool isFlickering;

    public Combatant Model { get; set; }

    public void Init() {
        Model = GetComponent<Combatant>();
        sprite.sprite = Data.sprites[Model.spriteId];
        UpdateHealthText();
        Model.CurrentHp.OnUpdate += UpdateHealthText;
        Model.MaxHp.OnUpdate += UpdateHealthText;
        Model.Animation.OnNewValue += PlayAnimation;
    }

    public void OnDestroy() {
        Model.CurrentHp.OnUpdate -= UpdateHealthText;
        Model.MaxHp.OnUpdate -= UpdateHealthText;
        Model.Animation.OnNewValue -= PlayAnimation;
    }

    private void Update() {
        if (isFlickering) {
            isFlickerOn = !isFlickerOn;
            sprite.color = isFlickerOn ? Color.black : Color.white;
        }
    }

    private void StopFlickering() {
        isFlickering = false;
        isFlickerOn = false;
        sprite.color = Color.white;
    }

    private void PlayAnimation(int animationIndex) {
        if (animationIndex == 1) {
            var offset = Model.isHero ? 0.5f : -0.5f;
            sprite.transform.localPosition = new Vector3(offset, 0, 0);
            Helper.DelayMethod(0.25f, () => sprite.transform.localPosition = new Vector3(0, 0, 0) );
            
        } else if (animationIndex == 2) {
            isFlickering = true;
            Helper.DelayMethod(0.5f, StopFlickering);
            
        } else if (animationIndex == 3) {
            sprite.color = Color.black;
        }
    }

    private void UpdateHealthText() {
        text.text = Model.CurrentHp.Value > 0 ? Model.CurrentHp.Value + " / " + Model.MaxHp.Value : ""; 
    }



}
