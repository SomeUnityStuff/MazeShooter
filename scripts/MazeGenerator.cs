using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeGenerator : MonoBehaviour
{
    public class Cell{
        public int left_wall_id;
        public int right_wall_id;
        public int top_wall_id;
        public int bot_wall_id;
        public int top_neighbour;
        public int right_neighbour;
        public int bot_neighbour;
        public int left_neighbour;
        public float x_center;
        public float y_center;
    }


    [SerializeField]
    GameObject VerticalWallPrefab;
    [SerializeField]
    GameObject HorizontalWallPrefab;
    [SerializeField]
    int ScreenToCellRatio;
    [SerializeField]
    float CellToWallRatio;

    float cell_x_size;
    float cell_y_size;
    float vert_wall_width;
    float vert_wall_height;
    float hor_wall_height;
    float hor_wall_width;

    List<Cell> cells;
    List<GameObject> hor_walls;
    List<GameObject> vert_walls;

    #region Properties
    public float CellXSize{
        get => cell_x_size;
        //set => cell_x_size = value;
    }

    public float CellYSize{
        get => cell_y_size;
        //set => cell_y_size = value;
    }

    public float VertWallWidth{
        get => vert_wall_width;
        //set => vert_wall_width = value;
    }

    public float VertWallHeight{
        get => vert_wall_height;
        //set => vert_wall_height = value;
    }
    public float HorWallWidth{
        get => hor_wall_width;
        //set => hor_wall_width = value;
    }

    public float HorWallHeight{
        get => hor_wall_height;
        //set => hor_wall_height = value;
    }

    public List<Cell> GetCells{
        get => cells;
    }

    public List<GameObject> GetHorizontalWalls{
        get => hor_walls;
    }

    public List<GameObject> GetVerticalWalls{
        get => vert_walls;
    }
    #endregion

    GameObject MakeHorizontalWall(Vector3 position, float x_size, float y_size){
        /*SIZES ARE IN SCREEEN REFERENCE*/
        GameObject new_wall = Instantiate<GameObject>(HorizontalWallPrefab, position, Quaternion.identity);
        Vector2 init_wall_size = new_wall.GetComponent<SpriteRenderer>().sprite.rect.size;
        float wall_x_scale = x_size / init_wall_size.x;
        float wall_y_scale = y_size / init_wall_size.y;
        new_wall.transform.localScale = new Vector3(wall_x_scale, wall_y_scale, 1.0f);
        return new_wall;
    }

    GameObject MakeVerticalWall(Vector3 position, float x_size, float y_size){
        /*SIZES ARE IN SCREEEN REFERENCE*/
        GameObject new_wall = Instantiate<GameObject>(VerticalWallPrefab, position, Quaternion.identity);
        Vector2 init_wall_size = new_wall.GetComponent<SpriteRenderer>().sprite.rect.size;
        float wall_x_scale = x_size / init_wall_size.x;
        float wall_y_scale = y_size / init_wall_size.y;
        new_wall.transform.localScale = new Vector3(wall_x_scale, wall_y_scale, 1.0f);
        return new_wall;
    }

    Cell MakeCell(float x_center, float y_center){
        Cell res_cell = new Cell();
        res_cell.x_center = x_center;
        res_cell.y_center = y_center;
        return res_cell;
    }

    void MarkNeighbours(){
        for (int i = 0; i < cells.Count; i++){
            cells[i].bot_neighbour = i % ScreenToCellRatio != 0 ? i-1 : -1;
            cells[i].top_neighbour = (i+1)%ScreenToCellRatio!=0 ? i+1: -1;
            cells[i].right_neighbour = (i < cells.Count - ScreenToCellRatio) ? i+ScreenToCellRatio : -1;
            cells[i].left_neighbour = (i>ScreenToCellRatio-1) ? i-ScreenToCellRatio : -1;
        }
    }

    List<int> GetExistingNeighbourList(Cell current_cell){
        List<int> neighbours = new List<int>();
        if (current_cell.bot_neighbour > 0){
            neighbours.Add(current_cell.bot_neighbour);
        }
        if (current_cell.top_neighbour > 0){
            neighbours.Add(current_cell.top_neighbour);
        }
        if (current_cell.right_neighbour > 0){
            neighbours.Add(current_cell.right_neighbour);
        }
        if (current_cell.left_neighbour > 0){
            neighbours.Add(current_cell.left_neighbour);
        }
        return neighbours;
    }

    List<int> FilterVisitedNeighbours(List<int> possible_neighbours, List<int> visited){
        List<int> filtered = new List<int>();
        for(int i=0; i<possible_neighbours.Count; i++){
            if (!visited.Contains(possible_neighbours[i])){
                filtered.Add(possible_neighbours[i]);
            }
        }
        return filtered;
    }

    void DestroyWallBetween(int start_id, int end_id){
        if (cells[start_id].bot_neighbour == end_id){
            int wall_id = cells[start_id].bot_wall_id;
            Destroy(hor_walls[wall_id]);
            return;
        }
        if (cells[start_id].top_neighbour == end_id){
            int wall_id = cells[start_id].top_wall_id;
            Destroy(hor_walls[wall_id]);
            return;
        }
        if (cells[start_id].left_neighbour == end_id){
            int wall_id = cells[start_id].left_wall_id;
            Destroy(vert_walls[wall_id]);
            return;
        }
        if (cells[start_id].right_neighbour == end_id){
            int wall_id = cells[start_id].right_wall_id;
            Destroy(vert_walls[wall_id]);
            return;
        }
        print("MAZE_GENERATION -- I build trace between NON neighbours cells!! their id are start=" + start_id + " and end=" + end_id);
        throw new Exception("BAD MAZE BUILD");
    }

    void BuildMaze(){
        List<int> visited = new List<int>();
        visited.Add(0);
        int curr_cell_id = 0;
        Stack<int> path_trace = new Stack<int>();
        path_trace.Push(0);
        while (visited.Count < cells.Count){
            List<int> possible_neighbours = GetExistingNeighbourList(cells[curr_cell_id]);
            possible_neighbours = FilterVisitedNeighbours(possible_neighbours, visited);
            if (possible_neighbours.Count == 0){
                curr_cell_id = path_trace.Pop();
                continue;
            }
            int next_cell_id = possible_neighbours[UnityEngine.Random.Range(0, possible_neighbours.Count)];
            DestroyWallBetween(curr_cell_id, next_cell_id);
            path_trace.Push(next_cell_id);
            visited.Add(next_cell_id);
            curr_cell_id = next_cell_id;
        }

    }

    void GenerateCells(){
        cells = new List<Cell>();
        //float curr_x_coor = 0.5f * cell_x_size;
        for(int i=0; i<ScreenToCellRatio; i++){
            float curr_x_coor = (i + 0.5f) * cell_x_size;
            for(int j=0; j<ScreenToCellRatio; j++){
                float curr_y_coor = (j + 0.5f) * cell_y_size;
                cells.Add(MakeCell(curr_x_coor, curr_y_coor));
            }
        }
        //MARKING IS BASED ON THE ORDER OF CELL CREATION!
        MarkNeighbours();
    }

    void GenerateHorizontalWalls(){
        hor_walls = new List<GameObject>();
        for (int i = 0; i < ScreenToCellRatio; i++){
            float curr_x_coor = (i + 0.5f) * cell_x_size;
            for (int j = 0; j < ScreenToCellRatio + 1; j++){
                float curr_y_coor = j * cell_y_size;
                Vector3 wall_pos = new Vector3(curr_x_coor, curr_y_coor, -Camera.main.transform.position.z);
                hor_walls.Add(MakeHorizontalWall(Camera.main.ScreenToWorldPoint(wall_pos), hor_wall_width, hor_wall_height));
            }
        }
        //MARKING IS BASED ON THE ORDER OF CELL CREATION!
        for (int i=0; i<cells.Count; i++){
            cells[i].bot_wall_id = i + i / ScreenToCellRatio;
            cells[i].top_wall_id = i + 1 + i / ScreenToCellRatio;
        }
    }

    void GenerateVerticalWalls(){
        vert_walls = new List<GameObject>();
        for (int i = 0; i < ScreenToCellRatio+1; i++){
            float curr_x_coor = i * cell_x_size;
            for (int j = 0; j < ScreenToCellRatio; j++){
                float curr_y_coor = (j+0.5f) * cell_y_size;
                Vector3 wall_pos = new Vector3(curr_x_coor, curr_y_coor, -Camera.main.transform.position.z);
                vert_walls.Add(MakeHorizontalWall(Camera.main.ScreenToWorldPoint(wall_pos), vert_wall_width, vert_wall_height));
            }
        }
        //MARKING IS BASED ON THE ORDER OF CELL CREATION!
        for (int i = 0; i < cells.Count; i++){
            cells[i].left_wall_id = i;
            cells[i].right_wall_id = i + ScreenToCellRatio;
        }
    }

    // Start is called before the first frame update
    void Start(){
        //TODO ALL THIS SHOULD BE IN CONSTRUCTOR!!!!
        cell_x_size = 1.0f * Screen.width / ScreenToCellRatio;
        cell_y_size = 1.0f * Screen.height / ScreenToCellRatio;
        vert_wall_width = cell_x_size / CellToWallRatio;
        hor_wall_height = cell_y_size / CellToWallRatio;
        vert_wall_height = cell_y_size - 0.5f * hor_wall_height;
        hor_wall_width = cell_x_size - 0.5f * vert_wall_width;
        GenerateCells();
        GenerateHorizontalWalls();
        GenerateVerticalWalls();
        BuildMaze();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
