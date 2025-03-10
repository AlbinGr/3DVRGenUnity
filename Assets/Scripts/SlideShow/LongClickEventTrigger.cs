using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class LongClickEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    [ Tooltip( "How long must pointer be down on this object to trigger a long press" ) ]
    public float durationThreshold = 1.0f;

    public UnityEvent onLongPress = new UnityEvent();
    public string imageFilePath = "Images/chair.png";

    

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;

    void Update( ) {
        if ( isPointerDown && !longPressTriggered ) {
            if ( Time.time - timePressStarted > durationThreshold ) {
                longPressTriggered = true;
                onLongPress.Invoke();
            }
        }
    }

    public void OnPointerDown( PointerEventData eventData ) {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;
    }

    public void OnPointerUp( PointerEventData eventData ) {
        isPointerDown = false;
    }


    public void OnPointerExit( PointerEventData eventData ) {
        isPointerDown = false;
    }
}