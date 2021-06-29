using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatantView : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] TextMeshPro damage;
    [SerializeField] SpriteRenderer sprite;

    private bool isFlickerOn;
    private bool isFlickering;

    public Combatant Model { get; set; }

    public void Init() {
        Model = GetComponent<Combatant>();
        sprite.sprite = Data.sprites[Model.spriteId];
        UpdateHealthText();
        Model.CurrentHp.OnUpdate += UpdateHealthText;
        Model.CurrentHp.OnChange += HealthChange;
        Model.MaxHp.OnUpdate += UpdateHealthText;
        Model.Animation.OnNewValue += PlayAnimation;
    }

    public void OnDestroy() {
        Model.CurrentHp.OnUpdate -= UpdateHealthText;
        Model.CurrentHp.OnChange -= HealthChange;
        Model.MaxHp.OnUpdate -= UpdateHealthText;
        Model.Animation.OnNewValue -= PlayAnimation;
    }

    private void FixedUpdate() {
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
            var offset = Model.IsHero ? 0.5f : -0.5f;
            sprite.transform.localPosition = new Vector3(offset, 0, 0);
            Helper.DelayMethod(0.25f, () => sprite.transform.localPosition = new Vector3(0, 0, 0) );
            
        } else if (animationIndex == 2) {
            isFlickering = true;
            Helper.DelayMethod(0.75f, StopFlickering);
            
        } else if (animationIndex == 3) {
            isFlickering = true;
            Helper.DelayMethod(0.75f, StopFlickering);
            Helper.DelayMethod(0.75f, () => sprite.color = Color.black);
        }
    }

    private void HealthChange(int before, int after) {        
        if (after <= before) {
            damage.color = Color.white;
            damage.text = (before - after).ToString();
        } else {
            damage.color = Color.green;
            damage.text = (after - before).ToString();
        }
        Helper.DelayMethod(0.75f, () => damage.text = "");

    }

    private void UpdateHealthText() {
        text.text = Model.CurrentHp.Value > 0 ? Model.CurrentHp.Value + " / " + Model.MaxHp.Value : ""; 
    }



}
