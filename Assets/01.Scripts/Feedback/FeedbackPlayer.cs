using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour
{
    private List<Feedback> _feedbackList;
    private void Awake()
    {
        _feedbackList = new List<Feedback>();
        GetComponents(_feedbackList);
        _feedbackList.ForEach(f => f.Init(this.transform));
    }
    
    public void CreateFeedback()
    {
        _feedbackList.ForEach(f => f.CreateFeedback());
    }
    public void FinishFeedback()
    {
        _feedbackList.ForEach(f => f.FinishFeedback());
    }
}
