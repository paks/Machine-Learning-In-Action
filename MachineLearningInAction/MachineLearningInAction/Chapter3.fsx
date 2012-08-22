#load "DecisionTrees.fs"
open MachineLearning.DecisionTrees

// Construct a Decision Tree by hand
let manualTree = 
    Choice 
        ("Sci-Fi", 
            Map [("No", 
                    Choice ("Action", 
                        Map [("Yes", Conclusion "Stallone");
                             ("No", Conclusion "Schwarzenegger")]));
                 ("Yes", Conclusion "Schwarzenegger")])

// Use the tree to Classify a test Subject
let test = Map [ ("Action", "Yes"); ("Sci-Fi", "Yes") ]
let actor = classify test manualTree

// Sample dataset
let movies =
    [| "Action"; "Sci-Fi"; "Actor" |],
    [| [| "Yes"; "No";  "Stallone" |];
       [| "Yes"; "No";  "Stallone" |];
       [| "No";  "No";  "Schwarzenegger"  |];
       [| "Yes"; "Yes"; "Schwarzenegger"  |];
       [| "Yes"; "Yes"; "Schwarzenegger"  |] |]

// Construct the Decision Tree off the data
// and classify another test subject
let tree = build movies
let subject = Map [ ("Action", "Yes"); ("Sci-Fi", "No") ]
let answer = classify subject tree

// Lenses dataset: http://archive.ics.uci.edu/ml/datasets/Lenses
// The following code assumes the dataset has been massaged a bit,
// and that data has been transformed to be comma-separated instead of tab.

open System.Net
open System.Text.RegularExpressions

let dowloadDataSet (url : string) separator =
    let client = new WebClient()
    client.DownloadString(url).Split('\n') 
    |> Array.filter((<>) "")
    |> Array.map (fun line -> Regex.Split(line, separator))

let lenses = 
    let dataset = 
        dowloadDataSet "http://archive.ics.uci.edu/ml/machine-learning-databases/lenses/lenses.data" "\\s+"
        |> Array.map (fun line -> 
            [| line.[1]
               line.[2]; 
               line.[3]; 
               line.[4];
               line.[5]|])
    let labels = [| "Age"; "Prescription Type"; "Astigmatic"; "Tears Production Rate"; "Decision" |]
    labels, dataset

let lensesTree = build lenses
// Nursery Dataset: http://archive.ics.uci.edu/ml/datasets/Nursery

let nursery =
    let labels = [| "parents"; "has_nurs"; "form"; "children"; "housing"; "finance"; "social"; "health"; "Decision" |]
    let dataset = 
        dowloadDataSet "http://archive.ics.uci.edu/ml/machine-learning-databases/nursery/nursery.data" ","
    labels, dataset

let nurseryTree = build nursery


#I @"C:\Program Files (x86)\Microsoft\Microsoft Automatic Graph Layout\bin"
#r "Microsoft.Msagl"
#r "Microsoft.Msagl.Drawing"
#r "Microsoft.Msagl.GraphViewerGdi"
open System
open System.Windows.Forms
open Microsoft.Msagl
open Microsoft.Msagl.Drawing
open Microsoft.Msagl.GraphViewerGdi

let tree2graph (graph : Microsoft.Msagl.Drawing.Graph) tree =
    let rec loop parent edgeLabel t =
        match t with
        | Conclusion(c) -> 
            let edge = graph.AddEdge(parent, c)
            edge.LabelText <- edgeLabel
            edge.Attr.Color <- Microsoft.Msagl.Drawing.Color.Green 
            let node = graph.FindNode(c)
            node.Attr.Shape <- Shape.Circle
            node.Attr.FillColor <- Color.Green
        | Choice(label, map) -> 
            let label = label + "-" + edgeLabel
            graph.AddNode(label).Attr.Shape <- Microsoft.Msagl.Drawing.Shape.Diamond
            if(parent <> "") then
                graph.AddEdge(parent, label).LabelText <- edgeLabel
            for kvp in map do
                loop label kvp.Key kvp.Value
    loop "" "" tree

let form = new Form() 

//create a viewer object 
let viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer() 
//create a graph object 
let graph = new Microsoft.Msagl.Drawing.Graph("graph") 
graph.Label.Text <- "Lenses"
//create the graph content 
tree2graph graph lensesTree
//tree2graph graph nurseryTree
graph.Attr.NodeSeparation <- graph.Attr.NodeSeparation * 1. 
graph.Attr.LayerSeparation <- graph.Attr.LayerSeparation / 2. 
//bind the graph to the viewer 
viewer.Graph <- graph 
//associate the viewer with the form 
form.SuspendLayout() 
viewer.Dock <- System.Windows.Forms.DockStyle.Fill 
form.Controls.Add(viewer) 
form.ResumeLayout() 
//show the form 
form.ShowDialog() 
