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
    float ScreenToCellRatio;
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

    int FindBotNeighbour(float this_x, float this_y, List<Cell> cells){
        int idx = -1;
        for(int i=0; i<cells.Count; i++){
            if(Mathf.Abs(cells[i].x_center - this_x)< 1.0e-3f &&
               Mathf.Abs(cells[i].y_center - this_y + cells[i].y_size) < 1.0e-3f)
            {
                idx = i;
            }
        }
        return idx;
    }

    int FindRightNeighbour(float this_x, float this_y, List<Cell> cells){
        int idx = -1;
        for (int i = 0; i < cells.Count; i++){
            if (Mathf.Abs(cells[i].x_center - this_x - cells[i].x_size) < 1.0e-3f &&
               Mathf.Abs(cells[i].y_center - this_y) < 1.0e-3f)
            {
                idx = i;
            }
        }
        return idx;
    }

    int FindTopNeighbour(float this_x, float this_y, List<Cell> cells){
        int idx = -1;
        for (int i = 0; i < cells.Count; i++){
            if (Mathf.Abs(cells[i].x_center - this_x) < 1.0e-3f &&
               Mathf.Abs(cells[i].y_center - this_y - cells[i].y_size) < 1.0e-3f)
            {
                idx = i;
            }
        }
    return idx;
    }

    int FindLeftNeighbour(float this_x, float this_y, List<Cell> cells){
        int idx = -1;
        for (int i = 0; i < cells.Count; i++){
            if (Mathf.Abs(cells[i].x_center - this_x + cells[i].x_size) < 1.0e-3f &&
               Mathf.Abs(cells[i].y_center - this_y) < 1.0e-3f){
                idx = i;
            }
        }
        return idx;
    }

    void MarkNeighbours(List<Cell> cells){
        for(int i=0; i<cells.Count; i++){
            cells[i].bot_neighbour = FindBotNeighbour(cells[i].x_center, cells[i].y_center, cells);
            cells[i].right_neighbour = FindRightNeighbour(cells[i].x_center, cells[i].y_center, cells);
            cells[i].top_neighbour = FindTopNeighbour(cells[i].x_center, cells[i].y_center, cells);
            cells[i].left_neighbour = FindLeftNeighbour(cells[i].x_center, cells[i].y_center, cells);
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
        float curr_x_coor = 0.5f * cell_x_size;
        while (curr_x_coor < Screen.width)
        {
            float curr_y_coor = 0.5f * cell_y_size;
            while (curr_y_coor < Screen.height)
            {
                cells.Add(MakeCell(curr_x_coor, curr_y_coor));
                curr_y_coor += cell_y_size;
            }
            curr_x_coor += cell_x_size;
        }
        return cells;
    }

    // Start is called before the first frame update
    void Start()
    {

        List<Cell> cells = GenerateCells();
        MarkNeighbours(cells);
        BuildMaze(cells);

        /*
        float cell_x_size = 1.0f * Screen.width / ScreenToCellRatio;
        float cell_y_size = 1.0f * Screen.height / ScreenToCellRatio;
        float x = Screen.width / 2.0f;
        float y = Screen.height / 2.0f;
        MakeCell(x, y);
        //MakeCell(x + cell_x_size, y);
        //MakeCell(x, y + cell_y_size);
        Vector3 pos_1 = new Vector3(1, 1, -Camera.main.transform.position.z);
        Vector3 w_pos_1 = Camera.main.ScreenToWorldPoint(pos_1);
        print("POS_1 = " + pos_1);
        print("W_POS_1 = " + w_pos_1);
        Vector3 pos_2 = new Vector3(10, 10, -Camera.main.transform.position.z);
        Vector3 w_pos_2 = Camera.main.ScreenToWorldPoint(pos_2);
        print("POS_2 = " + pos_2);
        print("W_POS_2 = " + w_pos_2);
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
