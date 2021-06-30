using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatantView : MonoBehaviour {

    [SerializeField] TextMeshPro text;
    [SerializeField] TextMeshPro damage;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] List<GameObject> buffSlots;
    [SerializeField] GameObject buffPrefab;

    private bool isFlickerOn;
    private bool isFlickering;
    private Combatant model;
    private List<GameObject> buffs = new List<GameObject>();

    public void Init(Combatant model) {
        this.model = model;
        sprite.sprite = Data.sprites[this.model.spriteId];
        UpdateHealthText();
        this.model.CurrentHp.OnUpdate += UpdateHealthText;
        this.model.CurrentHp.OnChange += HealthChange;
        this.model.MaxHp.OnUpdate += UpdateHealthText;
        this.model.Animation.OnNewValue += PlayAnimation;
        this.model.Buffs.OnNewValue += UpdateBuffs;
    }

    public void OnDestroy() {
        model.CurrentHp.OnUpdate -= UpdateHealthText;
        model.CurrentHp.OnChange -= HealthChange;
        model.MaxHp.OnUpdate -= UpdateHealthText;
        model.Animation.OnNewValue -= PlayAnimation;
        model.Buffs.OnNewValue -= UpdateBuffs;
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
            var offset = model.IsHero ? 0.5f : -0.5f;
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
        text.text = model.CurrentHp.Value > 0 ? model.CurrentHp.Value + " / " + model.MaxHp.Value : ""; 
    }

    private void UpdateBuffs(List<Buff> list) {
        foreach (var go in buffs) {
            Destroy(go);
        }
        for (int i=0; i<list.Count; i++) {
            var go = Instantiate(buffPrefab, buffSlots[i].transform.position, Quaternion.identity, buffSlots[i].transform);
            buffs.Add(go);
            go.GetComponent<BuffView>().SetSprite(list[i].data.sprite);
        }
    }



}
