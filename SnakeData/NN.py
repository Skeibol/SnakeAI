
import numpy as np
from dataHandler import writeData
class Model:
    def __init__(self, lr, iterations,layer_size=256,output_size=10):
        self.lr = lr
        self.iterations = iterations
        self.layer_size = layer_size
        self.output_size = output_size

    def weight_init(self,input_size):
        W1 = np.random.rand(self.layer_size,input_size) - 0.5 #10,broj primjera
        b1 = np.random.rand(self.layer_size,1) - 0.5 #10 flat biasa za svaki neuron u hiiden sloju
        W2 = np.random.rand(self.output_size,self.layer_size) - 0.5 #10 weightova za svaki od 10 output neurona hidden sloja (10*10)
        b2 = np.random.rand(self.output_size,1) - 0.5 #10 flat biasa za output sloj
        return W1,b1,W2,b2  

    def ReLU(self,x):
        return np.maximum(x,0)

    def LReLU(self,x):
        return np.where(x > 0, x, x*0.01) 

    def BackwardsReLU(self,x):
        return x>0

    def BackwardsLReLU(self,x):
        return np.where(x < 0, 0.01, 1.0) 

    def softmax(self,x): #Normalizacija output sloja na 0.0-1.0
        exp = np.exp(x - np.max(x)) 
        return exp / exp.sum(axis=0)
    def fakemax(self,x): #Normalizacija output sloja na 0.0-1.0
        exp = np.exp(x - np.max(x)) 
        return exp 
        


    def oneHot(self,Y):
        one_hot_Y = np.zeros((Y.size, Y.max() + 1))
        one_hot_Y[np.arange(Y.size), Y] = 1
        one_hot_Y = one_hot_Y.T
        return one_hot_Y #oneStolenEncoder


    def forward(self,X,W1,b1,W2,b2):
        Z1 = W1.dot(X) + b1 #(10, broj primjera) , dodajemo bias na svaki primjerak stupac po stupac 
        A1 = self.ReLU(Z1) # 10, broj primjera
        Z2 = W2.dot(A1) + b2 #10, broj primjera
        A2 = self.softmax(Z2) #10, broj primjera
        return Z1, A1, Z2, A2


    def backward(self,Z1, A1, Z2, A2, W2):
        m = self.Y.size
        hot_Y = self.oneHot(self.Y)
        err_Z2 = A2 - hot_Y
        err_W2 = 1/m * err_Z2.dot(A1.T)
        err_b2 = 1/m * np.sum(err_Z2)
        err_Z1 = W2.T.dot(err_Z2) * self.BackwardsReLU(Z1)
        err_W1 = 1/m * err_Z1.dot(self.X.T)
        err_b1 = 1/m * np.sum(err_Z1)

        return err_W1, err_b1, err_W2, err_b2

    def update(self,W1 , b1 , W2 , b2 ,err_W1, err_b1, err_W2, err_b2):
        W1 = W1 - self.lr * err_W1
        b1 = b1 - self.lr * err_b1
        W2 = W2 - self.lr * err_W2
        b2 = b2 - self.lr * err_b2

        return W1, b1, W2, b2

    def get_predictions(self,A2): #A2 je zadnji sloj u mrezi, daje prediction u one hot encoded obliku npr [0.2,0.1,0.5,0.3.....broj izlaznih neurona] * broj slika/podataka
        return np.argmax(A2, 0) #Unencoded

    def get_accuracy(self,predictions, Y):     
        return np.sum(predictions == Y) / Y.size #Svaki pogodak dodaje 1, promašaj 0, na kraju se podjeli sa velicinom podataka i dobije prosjecna tocnost


    def fit(self, X, Y,diagnostics=True,plot=False,save_best_model=False):  # Glavna funkcija koja poziva ostale metode
        self.X = X
        self.Y = Y
        size , m = X.shape
        W1, b1, W2, b2 = self.weight_init(size)
        prevAccuracy = 0.0
        for i in range(self.iterations):
            Z1, A1, Z2, A2 = self.forward(X,W1, b1, W2, b2)
            dW1, db1, dW2, db2 = self.backward(Z1, A1, Z2, A2, W2)
            W1, b1, W2, b2 = self.update(W1, b1, W2, b2, dW1, db1, dW2, db2)
            predictions = self.get_predictions(A2)
            accuracy = self.get_accuracy(predictions, self.Y)
            if accuracy>0.75 and accuracy>prevAccuracy and save_best_model:
                
                writeData((W1,b1,W2,b2))
                prevAccuracy=accuracy
            if diagnostics:
                ################################ DEBUGGING ###########################
                if i % 100 == 0: 
                    print("\rIteration: ", i)
                    print(np.asarray(((np.unique(predictions, return_counts=True)))).T)
                    print(np.asarray(((np.unique(Y, return_counts=True)))).T)
                    
                    print("Accuracy: ", accuracy)
                    
                    print("------------------------\n")
                ################################ DEBUGGING ###########################

            
        print("Max acc: ", prevAccuracy)
        return W1, b1, W2, b2

    def evaluate(self,X,Y, W1, b1, W2, b2):

        _,_,_, A2 = self.forward(X,W1, b1, W2, b2)
        predictions = self.get_predictions(A2)
        return self.get_accuracy(predictions, Y)

    def predict(self,X,W1,b1,W2,b2):
        Z1 = W1.dot(X) + b1 #(10, broj primjera) , dodajemo bias na svaki primjerak stupac po stupac 
        A1 = self.ReLU(Z1) # 10, broj primjer
        Z2 = W2.dot(A1) + b2 #10, broj primjera
        A2 = self.softmax(Z2) #10, broj primjera
        
        return Z2

    def load(path):
        data = []
        with open(path) as file:
            lines = file.readlines()
            
        for i in lines:
            
            i = i.replace("\n","")
            
            data.append(i)
        data = np.array((data)).reshape(len(data),-1)
        return data




##################################################################################
# STRUKTURA
# (784,6000) -> (10,784) -> (10,784) 
#

#X_train, Y_train, X_test, Y_test = mnist.load_data()
#SCALE_FACTOR = 255 # BITNO radi normalizacije
#WIDTH = X_train.shape[1]
#HEIGHT = X_train.shape[2]
#################### FORMATIRANJE PODATAKA ZA MREZU ###############################
#
#print(X_train.shape) # Training dataset je 600000,28,28 - MREZA PRIMA FORMAT : (BROJ_INPUT_NEURONA*KOLICINA_PODATAKA) u nasem slucaju (748,60000)
#X_train = X_train.reshape(X_train.shape[0],WIDTH*HEIGHT).T / SCALE_FACTOR # flatten svih piksela i skaliranje (.T = Transpose(flip matrix))
#X_test = X_test.reshape(X_test.shape[0],WIDTH*HEIGHT).T  / SCALE_FACTOR
#print(X_train.shape) # <---------------------- input neuron mreze 
#model = Model(0.3,100)
#W1, b1, W2, b2 = model.fit(X_train, Y_train)
#
#eval = model.evaluate(X_test, Y_test, W1, b1, W2, b2)
#
#print("acc", eval)