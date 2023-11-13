using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class ItemAbillity : MonoBehaviour
{
    [Header("ScaleAndMassUp")]
    [SerializeField] private float _scaleRaiseValue = 2f;
    //[SerializeField] private float _massUpValue = 2f;
    [SerializeField] private float _scaleUpDuration = 0.7f;
    [SerializeField] private float _scaleRevertDuration = 0.7f;

    private PlayerBrain _brain;

    private float[] _skillRemainTimes = new float[(int)EItemType.COUNT];
    private bool[] _skillProgresss = new bool[(int)EItemType.COUNT];

    private event UnityAction _startActionFeedback; //스킬 효과 적용시 재생할 feedback.
    private event UnityAction _endActionFeedback; //스킬 효과 후 재생할 feedback.

    Vector3 originScale;
    private void Awake()
    {
        _brain = GetComponent<PlayerBrain>();
    }

    private void Start()
    {
        originScale = transform.localScale;
    }

    #region Skills

    public void UseSkill(EItemType _itemType, float skillTime)
    {
        _skillRemainTimes[(int)_itemType] = skillTime;
        _skillProgresss[(int)_itemType] = true;
        _startActionFeedback?.Invoke();

        switch (_itemType)
        {
            case EItemType.SCALEUP:
                ScaleUp();
                break;
            default:
                break;
        }
    }

    private void ScaleUp()
    {
        Vector3 currentScale = transform.localScale;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(currentScale * _scaleRaiseValue, _scaleUpDuration));
        seq.OnComplete(() => _endActionFeedback?.Invoke());
    }


    #endregion

    #region RevertSkills

    private void RevertSkillApply(EItemType _itemType)
    {
        switch (_itemType)
        {
            case EItemType.SCALEUP:
                RevertScale();
                break;
            default:
                break;
        }
    }

    private void RevertScale()
    {
        Vector3 targetScale = originScale;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originScale, _scaleRevertDuration).SetEase(DG.Tweening.Ease.Linear));
        seq.OnComplete(() => _endActionFeedback?.Invoke());
    }
    #endregion

    private void Update()
    {
        for(int i = 0; i < _skillRemainTimes.Length; i++)
        {
            if(_skillRemainTimes[i] < 0f )
            {
                _skillRemainTimes[i] = 0f;
                if (_skillProgresss[i] == true) //스킬 적용중인거였으면.
                {
                    _skillProgresss[i] = false;
                    RevertSkillApply((EItemType)i);
                }
            }
            _skillRemainTimes[i] -= Time.deltaTime;
        }
    }
}
