from NN import Model
from dataHandler import loadData, writeData, splitDataset
import numpy as np



#pathW1 = "W1.txt"
#pathW2 = "W2.txt"
#pathb1 = "b1.txt"
#pathb2 = "b2.txt"
#
#
#b1 = Model.load(pathb1)
#b2 = Model.load(pathb2)
#W1 = Model.load(pathW1)
#W2 = Model.load(pathW2)



data  = loadData(False)
print(data.shape)
data = data[data[:,9].argsort()]
data = data[:20000]

unique, counts = np.unique(data[:,9], return_counts=True)

print(dict(zip(unique, counts)))

X_train, Y_train, X_test , Y_test = splitDataset(data,True)

model = Model(0.5,5000,layer_size=256,output_size=5)
W1, b1, W2, b2 = model.fit(X_train.T, Y_train)
writeData((W1,b1,W2,b2))
