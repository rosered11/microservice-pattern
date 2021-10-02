# microservice-pattern

## Circut Breaker

**Implement**

ทำการ set Handle Result ไว้เมื่อ result ไม่ได้ตามที่ต้องการ จะถือว่า process นั้น fail แล้ว fail count จะเพิ่มขึ้น 
และถ้า failed count เพิ่มขึ้นถึงจำนวนที่ set ไว้ circuit breaker จะทำงาน

**Status circute breaker**

1. Close: สถานะนี้หมายถึง ตัว breaker ยังไม่เปิด ทำให้มีการเรียกใช้งาน function ต่างๆ ได้อยู่
2. Open: breaker ถูกเปิดแล้ว เมื่อมี request เข้า ตัว breaker จะ throw exception ออกไป เพือปกป้องกันเรียกใช้ function
3. HalfOpen: breaker กำลังจะถูกปิด ก่อนจะปิด breaker จะวิ่งเข้า สถานะนี้ก่อน และถ้าทำงานสำเร็จ ก็จะเพิ่ม success count 

## Circuit Reference

- [Circuit Breaker](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker)

## BulkHead

Implement

1. Set parallel สำหรับ task เพื่อแยกการใช้ Thread pool ออกจากการ
2. Set จำนวน Queue ที่มีได้ต่อ Thread pool 

หลังจาก นั้นถ้ามี process เข้ามามากเกินกว่า thread pool ที่มีจะรับไว้ สามารถ handle process หลังจาก bulkhead reject ได้
หลังจาก ผ่านการ handle reject bulkhead แล้ว task นั้นจะเข้าสู่ exception

## BulkHead Reference

- [BulkHead](https://docs.microsoft.com/en-us/azure/architecture/patterns/bulkhead)

# Reference

- [Microservice Pattern](https://medium.com/ascend-developers/ทำ-microservices-ให้ยืดหยุ่นและแข็งแกร่งยิ่งกว่าเดิม-ด้วย-spring-cloud-และ-netflix-hystrix-af14ba952c46)