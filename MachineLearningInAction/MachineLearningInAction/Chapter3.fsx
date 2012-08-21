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
    let labels = [| "Age"; "Presc."; "Astigm"; "Tears"; "Decision" |]
    labels, dataset

let lensesTree = build lenses
// Nursery Dataset: http://archive.ics.uci.edu/ml/datasets/Nursery

let nursery =
    let labels = [| "parents"; "has_nurs"; "form"; "children"; "housing"; "finance"; "social"; "health"; "Decision" |]
    let dataset = 
        dowloadDataSet "http://archive.ics.uci.edu/ml/machine-learning-databases/nursery/nursery.data" ","
    labels, dataset

let nurseryTree = build nursery