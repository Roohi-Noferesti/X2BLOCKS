using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMover : MonoBehaviour
{
    // Start is called before the first frame update
    private Block _block;
    private int _row, _column;
    private bool _isMoving = false; 
    private Vector2 _origin,_destination;
    [SerializeField]
    private float _speed;

    private void Awake()
    {
        _block = GetComponent<Block>();
           
    }
    
    
    public void MoveToFirstEmpty(int column,int newRow,Vector2 Destination)
    {
        GridManager.instance.GridBusy=true; 
        _block.IsActive = true;
        _isMoving=true;
        _destination = Destination;
        _row = newRow;
        _column = column;
        GridManager.instance.GridArray[_row, _column] = _block;
    }
    
    public void MoveToFirstEmpty(int column, int newRow, Vector2 Destination,int oldRow)
    {
        // Debug.Log(gameObject.GetInstanceID());
        GridManager.instance.GridBusy=true;
        
        _row = newRow;
        _column=column;
        _block.IsActive = true;
        _isMoving = true;
        _destination = Destination;
        GridManager.instance.GridArray[oldRow, column] = null;
        GridManager.instance.GridArray[_row, _column] = _block;
        
        //  StartCoroutine(Co_MoveToFirstEmpty(column, newRow, Destination, oldRow));

    }

    
    

    public bool IsMoving()
    {
        return _isMoving;   
    }
    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                _destination, Time.deltaTime * _speed);
            if(Vector2.Distance(transform.position, _destination)<0.01f)
            {
                GridManager.instance.GridBusy = false;
                transform.position = _destination;
                _isMoving=false;
                
                GridActionManager.instance.CheckForMerge();
                
            }

        }
    }
}
