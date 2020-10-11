using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject VerticalWallPrefab;
    [SerializeField]
    GameObject HorizontalWallPrefab;
    [SerializeField]
    int ScreenToCellRatio;
    [SerializeField]
    float CellToWallRatio;
   
    class Cell{
        public GameObject left_wall;
        public GameObject right_wall;
        public GameObject top_wall;
        public GameObject bot_wall;
        public int top_neighbour;
        public int right_neighbour;
        public int bot_neighbour;
        public int left_neighbour;
        //coordinates and sizes are expected to be in screen frame!
        public float x_center;
        public float y_center;
        public float x_size;   //include both wall thickness and free space
        public float y_size;   //include both wall thickness and free space
    }

    GameObject MakeHorizontalWall(Vector3 position, float x_size, float y_size){
        /*SIZES ARE IN SCREEEN REFERENCE*/
        GameObject new_wall = Instantiate<GameObject>(HorizontalWallPrefab, position, Quaternion.identity);
        Vector2 init_wall_size = new_wall.GetComponent<SpriteRenderer>().sprite.rect.size;
        float wall_x_scale = x_size / init_wall_size.x;
        float wall_y_scale = y_size / init_wall_size.y;
        //float wall_x_scale = Screen.width / (ScreenToCellRatio * cur_wall_size.x);    //cellsize/current_wall_size
        //float wall_y_scale = Screen.height /( ScreenToCellRatio * CellToWallRatio * cur_wall_size.y);
        new_wall.transform.localScale = new Vector3(wall_x_scale, wall_y_scale, 1.0f);
        return new_wall;
    }

    GameObject MakeVerticalWall(Vector3 position, float x_size, float y_size){
        /*SIZES ARE IN SCREEEN REFERENCE*/
        GameObject new_wall = Instantiate<GameObject>(VerticalWallPrefab, position, Quaternion.identity);
        Vector2 init_wall_size = new_wall.GetComponent<SpriteRenderer>().sprite.rect.size;
        float wall_x_scale = x_size / init_wall_size.x;
        float wall_y_scale = y_size / init_wall_size.y;
        //float wall_x_scale = Screen.width /( ScreenToCellRatio * CellToWallRatio * cur_wall_size.x);    
        //float wall_y_scale = Screen.height / (ScreenToCellRatio * cur_wall_size.y);
        new_wall.transform.localScale = new Vector3(wall_x_scale, wall_y_scale, 1.0f);
        return new_wall;
    }

    Cell MakeCell(float x_center, float y_center){
        Cell res_cell = new Cell();
        res_cell.x_center = x_center;
        res_cell.y_center = y_center;
        res_cell.x_size = 1.0f * Screen.width / ScreenToCellRatio;
        res_cell.y_size = 1.0f * Screen.height / ScreenToCellRatio;
        float vert_wall_width = res_cell.x_size / CellToWallRatio;
        float hor_wall_height = res_cell.y_size / CellToWallRatio;
        float vert_wall_height = res_cell.y_size - 2.0f * hor_wall_height;
        float hor_wall_width = res_cell.x_size - 2.0f * vert_wall_width;
        //BUILDING RIGHT WALL
        float r_x_coor = x_center + 0.5f * res_cell.x_size - vert_wall_width;
        float r_y_coor = res_cell.y_center;
        Vector3 r_wall_pos = new Vector3(r_x_coor, r_y_coor, -Camera.main.transform.position.z);
        res_cell.right_wall = MakeVerticalWall(Camera.main.ScreenToWorldPoint(r_wall_pos), vert_wall_width, vert_wall_height);
        //BUILDING LEFT WALL
        float l_x_coor = res_cell.x_center - 0.5f * res_cell.x_size + vert_wall_width;
        float l_y_coor = res_cell.y_center;
        Vector3 l_wall_pos = new Vector3(l_x_coor, l_y_coor, -Camera.main.transform.position.z);
        res_cell.left_wall = MakeVerticalWall(Camera.main.ScreenToWorldPoint(l_wall_pos), vert_wall_width, vert_wall_height);
        //BUILDING TOP WALL
        float t_y_coor = res_cell.y_center + 0.5f * res_cell.y_size - hor_wall_height;
        float t_x_coor = res_cell.x_center;
        Vector3 t_wall_pos = new Vector3(t_x_coor, t_y_coor, -Camera.main.transform.position.z);
        res_cell.top_wall = MakeHorizontalWall(Camera.main.ScreenToWorldPoint(t_wall_pos), hor_wall_width, hor_wall_height);
        //BUILDING BOT WALL
        float b_y_coor = res_cell.y_center - 0.5f * res_cell.y_size + hor_wall_height;
        float b_x_coor = res_cell.x_center;
        Vector3 b_wall_pos = new Vector3(b_x_coor, b_y_coor, -Camera.main.transform.position.z);
        res_cell.bot_wall = MakeHorizontalWall(Camera.main.ScreenToWorldPoint(b_wall_pos), hor_wall_width, hor_wall_height);
        //FOR TESTING
        //Vector3 pos = new Vector3(res_cell.x_center, res_cell.y_center, -Camera.main.transform.position.z);
        //MakeVerticalWall(Camera.main.ScreenToWorldPoint(pos), res_cell.x_size, res_cell.y_size);
        return res_cell;
    }

    void MarkNeighbours(List<Cell> cells){
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

    void DestroyWallBetween(List<Cell> cells, int start_id, int end_id){
        if (cells[start_id].bot_neighbour == end_id){
            Destroy(cells[start_id].bot_wall);
            Destroy(cells[end_id].top_wall);
            return;
        }
        if (cells[start_id].top_neighbour == end_id){
            Destroy(cells[start_id].top_wall);
            Destroy(cells[end_id].bot_wall);
            return;
        }
        if (cells[start_id].left_neighbour == end_id){
            Destroy(cells[start_id].left_wall);
            Destroy(cells[end_id].right_wall);
            return;
        }
        if (cells[start_id].right_neighbour == end_id){
            Destroy(cells[start_id].right_wall);
            Destroy(cells[end_id].left_wall);
            return;
        }
    }

    void BuildMaze(List<Cell> cells){
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
            int next_cell_id = possible_neighbours[Random.Range(0, possible_neighbours.Count)];
            DestroyWallBetween(cells, curr_cell_id, next_cell_id);
            path_trace.Push(next_cell_id);
            visited.Add(next_cell_id);
            curr_cell_id = next_cell_id;
        }

    }

    List<Cell> GenerateCells(){
        float cell_x_size = 1.0f * Screen.width / ScreenToCellRatio;
        float cell_y_size = 1.0f * Screen.height / ScreenToCellRatio;
        List<Cell> cells = new List<Cell>();
        //float curr_x_coor = 0.5f * cell_x_size;
        for(int i=0; i<ScreenToCellRatio; i++){
            float curr_x_coor = (i + 0.5f) * cell_x_size;
            for(int j=0; j<ScreenToCellRatio; j++){
                float curr_y_coor = (j + 0.5f) * cell_y_size;
                cells.Add(MakeCell(curr_x_coor, curr_y_coor));
            }
        }
        //MARKING IS BASED ON THE ORDER OF CELL CREATION!
        MarkNeighbours(cells);
        return cells;
    }

    // Start is called before the first frame update
    void Start()
    {

        List<Cell> cells = GenerateCells();
        //List<Cell> hor_walls = GenerateHorizontalWalls();
        //List<Cell> vert_walls = GenerateVerticalWalls();
        //MarkNeighbours(cells);
        BuildMaze(cells);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
